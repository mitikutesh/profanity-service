using NUnit.Framework;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Data.Repositories;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Test
{
    [TestFixture]
    public class FakeProfanityService : IProfanityService
    {
        public Task<bool> AddProfanityAsync(ProfanityDTO profanity)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ClearAsync()
        {
            return Task.FromResult(true);
        }

        public Task<bool> ClearAsync(Language language)
        {
            return Task.FromResult(true);
        }

        public (bool, List<string>, int, long)? ContainsProfanity(string term)
        {
            return new(true, new List<string>(), 0, 0);
        }

        public Task<List<string>> GetAllProfanitiesAsync(Language language)
        {
            return Task.FromResult(new List<string>());
        }

        public Task<bool> RemoveProfanityAsync(ProfanityDTO profanity)
        {
            return Task.FromResult(true);
        }
    }

    [TestFixture]
    public class FakeProfanityDb : IProfanityWord
    {
        //private List<string> Profanities = new List<string>();
        private List<string> profanitites = new List<string>() { "fuck" };

        public Task<bool> AddToProfanityAsync(ProfanityDTO profanity)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAllProfanityAsync()
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteProfanityAsync(ProfanityDTO profanityDTO)
        {
            var tes = new ProfanityDTO() { Language = Language.EN, ProfanityWord = new List<string> { "ass" } };
            return Task.FromResult(true);
        }

        public Task<ProfanityDTO> GetProfanityAsync(Guid id)
        {
            return Task.FromResult(new ProfanityDTO());
        }

        public Task<ProfanityDTO> GetProfanityByLanguageAsync(Language language)
        {
            return Task.FromResult(new ProfanityDTO());
        }
    }

   
}
