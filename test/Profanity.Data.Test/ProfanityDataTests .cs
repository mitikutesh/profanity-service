using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace Profanity.Data.Test
{
    [TestFixture]
    public class ProfanityDataTests
    {
        private IProfanityWord profanityword;
        IProfanityServiceDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ProfanityServiceDbContext>()
            .UseInMemoryDatabase(databaseName: "ProfanityDB")
            .Options;

            profanityword = new ProfanityWord(new ProfanityServiceDbContext(options));
        }


        [Test]
        public async Task AddToProfanityAsync_Should_Workd()
        {
            var response = await profanityword.AddToProfanityAsync(new ProfanityDTO { Language = Language.EN, ProfanityWord = new List<string> { "test" } });
            response.Should().Be(true);
        }

        [Test]
        public async Task DeleteAllProfanityAsync_Should_Workd()
        {
            var response = await profanityword.DeleteAllProfanityAsync();
            response.Should().Be(true);
        }

        [Test]
        public async Task DeleteProfanityAsync_Should_Work()
        {
            var response = await profanityword.DeleteProfanityAsync(new ProfanityDTO { Language = Language.EN, ProfanityWord = new List<string> { "test" } });
            response.Should().Be(true);
        }


        [Test]
        public async Task GetProfanityByLanguageAsync_Should_Workd()
        {
            var response = await profanityword.GetProfanityByLanguageAsync(Language.EN);
            Assert.That(() => response, Throws.Nothing);
        }


    }
}