using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.Service.Interfaces
{
    public interface IAccountService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
        AuthenticationResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        User GetById(int id);
        IEnumerable<User> GetAll();
        Task<bool> RegisterAsync(RegisterUser model);
    }
}
