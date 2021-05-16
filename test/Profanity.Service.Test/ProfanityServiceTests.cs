using NUnit.Framework;
using Profanity.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profanity.Service.Test
{
    [TestFixture]
    public class Tests
    {
        private List<string> profanitites = new List<string>() { "fuck" };
        private IProfanityService profanityService;

        [SetUp]
        public void Setup()
        {
            profanityService = new ProfanityService(new FakeProfanityDb());
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public async Task AddProfanity__Should_WorkAsync()
        {
            var result = await profanityService.AddProfanityAsync(new Data.DTO.ProfanityDTO());

            Assert.AreEqual(result, false);
        }

        [Test]
        public void Remove_Prfanity_Should_Work() { Assert.Pass(); }

        [Test]
        public void Clear_Should_Clear_All_Prfanity() { Assert.Pass(); }
    }
}