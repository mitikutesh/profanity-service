using Destructurama.Attributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Profanity.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [LogMasked]
        public string Username { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }

        [JsonIgnore]
        public List<RefreshToken> Refresh_Tokens { get; set; } = new List<RefreshToken>();
    }
}
