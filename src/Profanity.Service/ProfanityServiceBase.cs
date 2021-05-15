using Profanity.Data.DTO;
using Profanity.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.Service
{
    public class ProfanityServiceBase
    {

        protected List<string> _profanities;
        protected readonly IProfanityWord _profanityDataService;


        /// <summary>
        /// initializes existing profanity list.
        /// </summary>
        public ProfanityServiceBase(IProfanityWord profanityDataService)
        {
            _profanityDataService = profanityDataService;
            _ = initAsync();
        }

        private async Task initAsync()
        {
            var result = await _profanityDataService.GetPrfanityAsync(new Guid("5AFE2611-E655-4001-AEC6-798CFFC6F48F"));
            _profanities = result.ProfanityWord;
        }
        /// <summary>
        ///allows custom  profanities replacing the existing list.
        /// </summary>
        /// <param name="profanityList">List of words considered profanities.</param>
        protected ProfanityServiceBase(List<string> profanityList)
        {
            if (profanityList == null)
            {
                throw new ArgumentNullException(nameof(profanityList));
            }

            _profanities = profanityList;
        }

        /// <summary>
        /// Add profanity words to the existing words.
        /// </summary>
        /// <param name="profanity">Additional profanit to add.</param>
        public void AddProfanity(string profanity)
        {
            if (string.IsNullOrEmpty(profanity))
            {
                throw new ArgumentNullException(nameof(profanity));
            }

            _profanities.Add(profanity);
        }

        /// <summary>
        /// Add a custom list profanities to the existing list with out replacing the existing one.
        /// </summary>
        /// <param name="profanityList">Profanities to add.</param>
        public void AddProfanity(List<string> profanityList)
        {
            if (profanityList == null)
            {
                throw new ArgumentNullException(nameof(profanityList));
            }

            _profanities.AddRange(profanityList);
        }

        /// <summary>
        /// delete a profanity from the loaded list.
        /// </summary>
        /// <param name="profanity">The profanity to be removed.</param>
        /// <returns>True if profanity is removed. Otherwise False.</returns>
        public bool RemoveProfanity(string profanity)
        {
            if (string.IsNullOrEmpty(profanity))
            {
                throw new ArgumentNullException(nameof(profanity));
            }

            return _profanities.Remove(profanity.ToLower(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// delete list of profanities from the loaded list..
        /// </summary>
        /// <param name="profanities">prfanities to be removed.</param>
        /// <returns>True if the profanities is removed. otherwise False.</returns>
        public bool RemoveProfanity(List<string> profanities)
        {
            if (profanities == null)
            {
                throw new ArgumentNullException(nameof(profanities));
            }

            foreach (string naughtyWord in profanities)
            {
                if (!RemoveProfanity(naughtyWord))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// clear or delete current loaded list.
        /// </summary>
        public void Clear()
        {
            _profanities.Clear();
        }

        /// <summary>
        /// Countes and return number of profanities in a text.
        /// </summary>
        public int Count
        {
            get
            {
                return _profanities.Count;
            }
        }
    }
}
