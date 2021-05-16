using NUnit.Framework;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Data.Repositories;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.Service.Test
{
    [TestFixture]
    public class FakeProfanityDb : IProfanityWord
    {
        private List<string> Profanities = new List<string>();

        public Task<bool> AddToProfanityAsync(ProfanityDTO prospect)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAllProfanityAsync()
        {
            return Task.FromResult(true);
        }

        [TestCase()]
        [TestCase()]
        [TestCase()]
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