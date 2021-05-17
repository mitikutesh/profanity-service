using Profanity.Data.DTO;
using Profanity.Data.Repositories;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Profanity.Service
{
    public class ProfanityService : ProfanityServiceBase, IProfanityService
    {
        public ProfanityService(IProfanityWord profanityWord) : base(profanityWord)
        {
        }

        public ProfanityService(List<string> profanityList) : base(profanityList)
        {
        }

        public (bool, List<string>, int, long)? ContainsProfanity(string term)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (string.IsNullOrWhiteSpace(term))
            {
                return (false, null, 0, 0);
            }
            if (!_profanities.Any()) throw new Exception("no prfanity list set");
            List<string> potentialProfanities = _profanities.Where(word => word.Length <= term.Length).ToList();

            // We might have a very short phrase coming in, resulting in no potential matches even before the regex
            if (potentialProfanities.Count == 0)
            {
                return (false, null, 0, 0);
            }

            Regex regex = new Regex(string.Format(@"(?:{0})", string.Join("|", potentialProfanities).Replace("$", "\\$"), RegexOptions.IgnoreCase));

            bool isMatch = false;
            List<string> badwords = new List<string>();
            int counter = 0;
            foreach (Match profanity in regex.Matches(term))
            {
                isMatch = true;
                counter++;
                badwords.Add(profanity.Value);
            }
            badwords = badwords.Distinct().ToList();
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            return (isMatch, badwords, counter, elapsedTime);
        }
    }
}