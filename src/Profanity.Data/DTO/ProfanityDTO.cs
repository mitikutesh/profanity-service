using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Profanity.Data.DTO
{
    public class ProfanityDTO
    {
        public Language Language { get; set; }
        public List<string> ProfanityWord { get; set; }
    }
}
