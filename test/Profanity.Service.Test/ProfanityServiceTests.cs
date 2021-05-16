using NUnit.Framework;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profanity.Service.Test
{
    [TestFixture]
    public class Tests 
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void AddProfanity_Single_Word_Should_Work() { Assert.Pass();  }

        [Test]
        public void AddProfanity_WordList_Should_Work() { Assert.Pass(); }

        [Test]
        public void Clear_Should_Clear_All_Prfanity() { Assert.Pass(); }

    }

    public class FakeProfanityService : IProfanityService
    {
        public int Count => throw new System.NotImplementedException();

        public Task<bool> AddProfanityAsync(ProfanityDTO profanity)
        {
            throw new System.NotImplementedException();
        }

        public void ClearAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ClearAsync(Language language)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsProfanity(string term)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> FindAllProfanities(string sentence)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> FindAllProfanities(string sentence, bool removePartialMatches)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<string>> GetAllProfanitiesAsync(Language language)
        {
            throw new System.NotImplementedException();
        }

        public (int, int, string)? GetCompleteWord(string toCheck, string profanity)
        {
            throw new System.NotImplementedException();
        }

        public bool IsProfanity(string word)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveProfanityAsync(ProfanityDTO profanity)
        {
            throw new System.NotImplementedException();
        }

        Task<bool> IProfanityService.ClearAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}