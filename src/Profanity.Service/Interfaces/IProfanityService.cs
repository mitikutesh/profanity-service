using System;
using System.Collections.Generic;
using System.Text;

namespace Profanity.Service.Interfaces
{
    public interface IProfanityService
    {
        bool IsProfanity(string word);
        IEnumerable<string> FindAllProfanities(string sentence);
        IEnumerable<string> FindAllProfanities(string sentence, bool removePartialMatches);

        bool ContainsProfanity(string term);


        (int, int, string)? GetCompleteWord(string toCheck, string profanity);

        void AddProfanity(string profanity);
        void AddProfanity(List<string> profanityList);

        bool RemoveProfanity(string profanity);
        bool RemoveProfanity(List<string> profanities);

        void Clear();

        int Count { get; }
    }
}
