using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.API.Model
{
    public class ResponseModel
    {
        public bool IsProfanity { get; set; }
        public int Count { get; set; }
        public IEnumerable<string> FoundProfanityWords { get; set; }
        public long ElapasedTimeInMs { get; set; }
    }
}
