using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Profanity.Service.Helpers
{
    public static class Extension
    {
        public static Guid NewGuid()
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[16];
                provider.GetBytes(bytes);

                return new Guid(bytes);
            }
        }

        //TODO add more if needed
        public static AuthenticationResponse HideKeys(this AuthenticationResponse jwt)
        {
            if (jwt == null) return null;
            jwt.SessionId = null;
            return jwt;
        }

        public static string? GetClaim(string token, string tokenClaimType)
        {
            var claims = GetClaims(token);
            if (claims.Any())
            {
                Claim claim = string.IsNullOrWhiteSpace(tokenClaimType) ? claims.FirstOrDefault() : claims.Where(c => c.Type == tokenClaimType).FirstOrDefault();

                return claim?.Value;
            }

            return null;
        }
        public static IEnumerable<Claim>? GetClaims(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenInfo = tokenHandler.ReadToken(token);

                // var utcdateTime = ((JwtSecurityToken)tokenInfo);
                var claims = ((JwtSecurityToken)tokenInfo).Claims;
                return claims;
            }
            return null;
        }
    }

}
