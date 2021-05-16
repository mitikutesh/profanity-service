using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Profanity.API.Helper;
using Profanity.Data.Entities;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Test
{
    public class APIWebApplicationFactory : WebApplicationFactory<Startup>
    {
    }
    [TestFixture]
    public class ProfanityControllerTests
    {
        private APIWebApplicationFactory _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void GivenARequestToTheController()
        {
            _factory = new APIWebApplicationFactory();
            _client = _factory.CreateClient();
        }


        private static string GetPath(string fileName)
            => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

        [Test]
        [TestCase("TestData.txt", Language.EN)]
        public async Task Check_If_Text_Has_Profanity_Should_Return_Sucess(string txtName, Language language)
        {
            var upfilebytes = File.ReadAllBytes(GetPath(txtName));

            var json = JsonConvert.SerializeObject( language );

            MultipartFormDataContent content = new MultipartFormDataContent();
            ByteArrayContent baContent = new ByteArrayContent(upfilebytes);
            content.Add(baContent, "files", "03-0302-M0018_6464_He_5.txt");
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            content.Add(stringContent, "language");

            var response = await _client.PostAsync($"/api/Profanity/{EndPoints.CheckProfanity}", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Add_Profanity_Should_work()
        {
            var result = await _client.GetAsync($"/api/Profanity/{EndPoints.GetProfanitites}?language=EN");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Remove_WordsFrom_Profanity_List_Should_work()
        {
            var result = await _client.GetAsync($"/api/Profanity/{EndPoints.GetProfanitites}?language=EN");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Get_Profanity_List_By_Language_Should_Retrieve_Profanities()
        {
            var result = await _client.GetAsync($"/api/Profanity/{EndPoints.GetProfanitites}?language=EN");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    
        [Test]
        public async Task Clear_All_Profanities_Should_Remove_All_Prfanities()
        {
            var result = await _client.GetAsync($"/api/Profanity/{EndPoints.GetProfanitites}?language=EN");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}