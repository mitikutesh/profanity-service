using Destructurama.Attributed;
using System;
using System.Collections.Generic;
using System.Text;

namespace Profanity.Data.DTO
{
    public class RegisterUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [LogMasked]
        public string Username { get; set; }
        [LogMasked]
        public string Password { get; set; }
    }
}
