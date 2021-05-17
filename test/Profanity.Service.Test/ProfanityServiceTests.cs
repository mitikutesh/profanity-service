using FluentAssertions;
using NUnit.Framework;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profanity.Service.Test
{
    [TestFixture]
    public class ProfanityServiceTests 
    {
        private List<string> profanitites = new List<string>() { "fuck" };
        private IProfanityService profanityService;

        [SetUp]
        public void Setup()
        {
            profanityService = new ProfanityService(new FakeProfanityDb());
        }

      

        [Test]
        public async Task ContainsProfanity_Should_WordAsync(string term)
        {
            var val = (true, new List<string>(), 0, 0);
            var result =  profanityService.ContainsProfanity("test if this test text has profanity");
            result.Should().Equals(val);
        }

        [Test]
        public async Task AddProfanityAsync_Sould_Work()
        {
            var result = await profanityService.AddProfanityAsync(new ProfanityDTO() { });
            result.Should().BeTrue();
        }

        [Test]
        public async Task RemoveProfanityAsync_Should_Word()
        {
            var result = await profanityService.RemoveProfanityAsync(new ProfanityDTO() { });
            result.Should().BeTrue();
        }

        [Test]
        public async Task ClearAsync_Should_Word()
        {
            var result = await profanityService.ClearAsync();
                result.Should().BeTrue();
        }

        [Test]
        public async Task GetAllProfanitiesAsync_Should_Word()
        {

            var result = await profanityService.GetAllProfanitiesAsync(Language.EN);
            result.Should().BeOfType<List<string>>();
        }

        //public (bool, List<string>, int, long)? ContainsProfanity(string term)
    }
}