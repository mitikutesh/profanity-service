using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Profanity.Data;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service.Helpers;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.Service
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly JwtOptions _jwtOptions;
        private readonly JwtHeader _jwtHeader;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly IMapper _mapper;

        private ProfanityServiceDbContext _context;

        public AccountService(IOptions<JwtOptions> jwtOptions, ILogger<AccountService> logger, ProfanityServiceDbContext context, IMapper mapper)
        {
            _logger = logger;
            _jwtOptions = jwtOptions.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_signingCredentials);
            _context = context;
            _mapper = mapper;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            _logger.LogDebug("Authenticating a user");
            //  get a user if user exist create token for that user
            var user = _context.Api_Users.SingleOrDefault(a => a.Username == request.Username);
            if (user == null) return null;
            if (!CryptoService.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
                return null;
            var jwtToken = CreateJwtToken(user);
            var refreshToken = CreateRefreshToken(ipAddress);

            user.Refresh_Tokens.Add(refreshToken);
            _context.Update(user);
            await _context.SaveChangesAsync();

            var response = new AuthenticationResponse(user, jwtToken, refreshToken.Token);
            if (_jwtOptions.KeepSecretValues)
                return response.HideKeys();

            return response;
        }

        public AuthenticationResponse? RefreshToken(string token, string ipAddress)
        {
            _logger.LogDebug("Refreshing token");
            var user = _context.Api_Users.SingleOrDefault(u => u.Refresh_Tokens.Any(t => t.Token == token));
            if (user == null) return null;

            var refreshToken = user.Refresh_Tokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = CreateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.Refresh_Tokens.Add(newRefreshToken);

            //TODO
            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = CreateJwtToken(user);

            var response = new AuthenticationResponse(user, jwtToken, newRefreshToken.Token);
            if (_jwtOptions.KeepSecretValues)
                return response.HideKeys();
            return response;
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            _logger.LogDebug("Revoking token.");
            var user = _context.Api_Users.SingleOrDefault(u => u.Refresh_Tokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.Refresh_Tokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<User> GetAll()
        => _context.Api_Users;

        public User GetById(int id)
        {
            return _context.Api_Users.Find(id);
        }

        private RefreshToken CreateRefreshToken(string ipAddress)
        {
            _logger.LogDebug($"Creating refresht for the ip {ipAddress} ");
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        public string CreateJwtToken(User user)
        {
            _logger.LogDebug("Creating Jwt Token");
            var sessionId = Extension.NewGuid().ToString();
            //var nowUtc = DateTime.UtcNow;
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.UtcNow;
            var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
            var exp = (int)issueTime.AddMinutes(_jwtOptions.ExpiryMinutes).Subtract(utc0)
                .TotalSeconds;

            var expires = issueTime.AddMinutes(_jwtOptions.ExpiryMinutes);
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var payload = new JwtPayload
            {
                {"sub", user.Username},
                {"iss", _jwtOptions.Issuer},
                {"iat", iat},
                {"exp", exp},
                {"unique_name", user.Username}
            };
            //TODO check required claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(CustomClaimTypes.UserId, $"{user.Username}", "GUID"),  //TODO
                //new Claim(ClaimTypes.Role, user.Role), //TODO
                new Claim(CustomClaimTypes.SessionId, $"{sessionId}", "GUID"),
                //TOD TEST
            };

            payload.AddClaims(claims);

            var jwt = new JwtSecurityToken(_jwtHeader, payload);

            var token = _jwtSecurityTokenHandler.WriteToken(jwt);
            return token;
        }

        public async Task<bool> RegisterAsync(RegisterUser model)
        {
            _logger.LogDebug("Registering a new user");
            try
            {
                var registredUser = await _context.Api_Users.Select(a => a.Username).SingleOrDefaultAsync(a => a.Equals(model.Username));
                if (!string.IsNullOrWhiteSpace(registredUser))
                {
                    _logger.LogInformation($"User already exist");
                    return false;
                }

                //register user
                var user = _mapper.Map<RegisterUser, User>(model);
                user.PasswordHash = CryptoService.GeneratePasswordHash(model.Password, out string salt);
                user.Salt = salt;
                var validuser = _context.Api_Users.Add(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering the user");
                throw;
            }
        }
    }
}
