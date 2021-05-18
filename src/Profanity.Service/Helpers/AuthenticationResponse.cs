using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Profanity.Service.Helpers
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }


        public int Expires { get; set; }
        public int MinutesInUse { get; set; }
        public string? SessionId { get; set; }

        public AuthenticationResponse(User user, string accessTOken, string refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            AccessToken = accessTOken;
            RefreshToken = refreshToken;
        }
    }
}
