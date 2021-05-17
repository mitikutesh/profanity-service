using Profanity.Data.DTO;
using Profanity.Data.Entities;
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
            var result = _profanityDataService.GetProfanityByLanguageAsync(Data.Entities.Language.EN).Result;
            if (result != null)
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
        public async Task<bool> AddProfanityAsync(ProfanityDTO profanity)
        {
            if (profanity == null)
            {
                throw new ArgumentNullException(nameof(profanity));
            }
            var response = await _profanityDataService.AddToProfanityAsync(profanity);
            return response;
        }

        public async Task<bool> RemoveProfanityAsync(ProfanityDTO profanity)
        {
            if (profanity == null)
            {
                throw new ArgumentNullException(nameof(profanity));
            }
            var response = await _profanityDataService.DeleteProfanityAsync(profanity);
            return response;
        }

        public async Task<List<string>> GetAllProfanitiesAsync(Language language)
        {
            var profanityDTO = await _profanityDataService.GetProfanityByLanguageAsync(language);
            return profanityDTO?.ProfanityWord;
        }

        /// <summary>
        /// clear or delete current loaded list.
        /// </summary>
        public async Task<bool> ClearAsync()
           => await _profanityDataService.DeleteAllProfanityAsync();

        public async Task<bool> ClearAsync(Language language)
       => await _profanityDataService.DeleteAllProfanityAsync();
    }
}