using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.Service.Helpers
{
    public class AuthenticationRequest
    {
        [Required] [DefaultValue("username")] public string Username { get; set; }

        [DefaultValue("password")] public string Password { get; set; }
    }
}
