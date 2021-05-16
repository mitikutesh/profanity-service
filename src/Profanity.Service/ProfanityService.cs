using Profanity.Data.DTO;
using Profanity.Data.Repositories;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Profanity.Service
{
    public class ProfanityService : ProfanityServiceBase, IProfanityService
    {
        private readonly IProfanityWord _profanityWord;

        public ProfanityService(IProfanityWord profanityWord) : base(profanityWord)
        {
            //var k = _profanityDataService.Get().Select(a => a.ProfanityWords);
            //var kk = _profanities;
        }

        public ProfanityService(List<string> profanityList) : base(profanityList)
        {
        }

        public bool IsProfanity(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }
            return _profanities.Contains(word.ToLower(CultureInfo.InvariantCulture));
        }

        public IEnumerable<string> FindAllProfanities(string sentence)
        {
            return FindAllProfanities(sentence, false);
        }

        public IEnumerable<string> FindAllProfanities(string sentence, bool removePartialMatches)
        {
            if (string.IsNullOrEmpty(sentence))
            {
                return default;
            }

            sentence = sentence.ToLower();
            sentence = sentence.Replace(".", "");
            sentence = sentence.Replace(",", "");

            var words = sentence.Split(' ');

            // Deduplicate any partial matches, ie, if the word "twatting" is in a sentence, don't include "twat" if part of the same word.
            if (removePartialMatches)
            {
                _profanities.RemoveAll(x => _profanities.Any(y => x != y && y.Contains(x)));
            }
            return FilterSwearListForCompleteWordsOnly(sentence).Distinct().ToList();
        }

        public (int, int, string)? GetCompleteWord(string toCheck, string profanity)
        {
            if (string.IsNullOrEmpty(toCheck))
            {
                return null;
            }

            string profanityLowerCase = profanity.ToLower(CultureInfo.InvariantCulture);
            string toCheckLowerCase = toCheck.ToLower(CultureInfo.InvariantCulture);

            if (toCheckLowerCase.Contains(profanityLowerCase))
            {
                var startIndex = toCheckLowerCase.IndexOf(profanityLowerCase, StringComparison.Ordinal);
                var endIndex = startIndex;

                // Work backwards in string to get to the start of the word.
                while (startIndex > 0)
                {
                    if (toCheck[startIndex - 1] == ' ' || char.IsPunctuation(toCheck[startIndex - 1]))
                    {
                        break;
                    }

                    startIndex -= 1;
                }

                // Work forwards to get to the end of the word.
                while (endIndex < toCheck.Length)
                {
                    if (toCheck[endIndex] == ' ' || char.IsPunctuation(toCheck[endIndex]))
                    {
                        break;
                    }

                    endIndex += 1;
                }

                return (startIndex, endIndex, toCheckLowerCase.Substring(startIndex, endIndex - startIndex).ToLower(CultureInfo.InvariantCulture));
            }

            return null;
        }

        public bool ContainsProfanity(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return false;
            }

            List<string> potentialProfanities = _profanities.Where(word => word.Length <= term.Length).ToList();

            // We might have a very short phrase coming in, resulting in no potential matches even before the regex
            if (potentialProfanities.Count == 0)
            {
                return false;
            }

            Regex regex = new Regex(string.Format(@"(?:{0})", string.Join("|", potentialProfanities).Replace("$", "\\$"), RegexOptions.IgnoreCase));

            bool isMatch = false;
            int counter = 0;
            foreach (Match profanity in regex.Matches(term))
            {
                isMatch = true;
                counter++;
               // return true;
            }

            //return false;
            return isMatch;
        }

        private StringBuilder CensorStringByProfanityList(char censorCharacter, List<string> swearList, StringBuilder censored, StringBuilder tracker, bool ignoreNumeric)
        {
            foreach (string word in swearList.OrderByDescending(x => x.Length))
            {
                (int, int, string)? result = (0, 0, "");
                var multiWord = word.Split(' ');

                if (multiWord.Length == 1)
                {
                    do
                    {
                        result = GetCompleteWord(tracker.ToString(), word);

                        if (result != null)
                        {
                            string filtered = result.Value.Item3;

                            if (ignoreNumeric)
                            {
                                filtered = Regex.Replace(result.Value.Item3, @"[\d-]", string.Empty);
                            }

                            if (filtered == word)
                            {
                                for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                                {
                                    censored[i] = censorCharacter;
                                    tracker[i] = censorCharacter;
                                }
                            }
                            else
                            {
                                for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                                {
                                    tracker[i] = censorCharacter;
                                }
                            }
                        }
                    }
                    while (result != null);
                }
                else
                {
                    censored = censored.Replace(word, CreateCensoredString(word, censorCharacter));
                }
            }

            return censored;
        }

        private List<string> FilterSwearListForCompleteWordsOnly(string sentence)
        {
            List<string> filteredSwearList = new List<string>();
            StringBuilder tracker = new StringBuilder(sentence);

            foreach (string word in _profanities.OrderByDescending(x => x.Length))
            {
                (int, int, string)? result = (0, 0, "");
                var multiWord = word.Split(' ');

                if (multiWord.Length == 1)
                {
                    do
                    {
                        result = GetCompleteWord(tracker.ToString(), word);

                        if (result != null)
                        {
                            if (result.Value.Item3.Contains(word))
                            {
                                filteredSwearList.Add(word);

                                for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                                {
                                    tracker[i] = '*';
                                }
                                break;
                            }

                            for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                            {
                                tracker[i] = '*';
                            }
                        }
                    }
                    while (result != null);
                }
                else
                {
                    filteredSwearList.Add(word);
                    tracker.Replace(word, " ");
                }
            }

            return filteredSwearList;
        }

        private static string CreateCensoredString(string word, char censorCharacter)
        {
            string censoredWord = string.Empty;

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] != ' ')
                {
                    censoredWord += censorCharacter;
                }
                else
                {
                    censoredWord += ' ';
                }
            }

            return censoredWord;
        }

        private void AddMultiWordProfanities(List<string> swearList, string postAllowListSentence)
        {
            swearList.AddRange(
                from string profanity in _profanities
                where postAllowListSentence.ToLower(CultureInfo.InvariantCulture).Contains(profanity)
                select profanity);
        }


    }
}
