using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Profanity.API;
using Profanity.API.Helper;
using Profanity.API.Model;
using Profanity.Data.Entities;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests
{
    public class APIWebApplicationFactory : WebApplicationFactory<Startup>
    {
    }
    [TestFixture]
    public class IntegrationTests
    {
        private APIWebApplicationFactory _factory;
        private HttpClient _client;
        private string version = "v1";

        [OneTimeSetUp]
        public void GivenARequestToTheController()
        {
            _factory = new APIWebApplicationFactory();
            var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
            _client = _factory.CreateClient(options);
        }


        private static string GetPath(string fileName)
            => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

        [Test, Order(1)]
        public async Task Add_Profanity_word_Tolist_Should_work()
        {
            var request = new RequestModel
            {
                Language = Language.EN,
                ProfanityWord = new List<string> { "fuck" }
            };
            var response = await _client.PutAsync($"/api/{version}/Profanity/{EndPoints.AddWordToProfanity}", ConvertObjectToStringContent(request));
            response.EnsureSuccessStatusCode();
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();
        }

        [Test, Order(2)]
        [TestCase(Language.EN)]
        public async Task Get_Profanity_List_By_Language_Should_Retrieve_Profanities(Language language)
        {
            var response = await _client.GetAsync($"/api/{version}/Profanity/{EndPoints.GetProfanitites}?language={language}");
            response.EnsureSuccessStatusCode();
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();
        }


        [Test, Order(3)]
        [TestCase("TestData.txt", Language.EN)]
        public async Task Check_If_Text_Has_Profanity_Should_Return_value(string txtName, Language language)
        {
            // Arrange
            var expectedContentType = "application/json";
            var url = $"/api/{version}/Profanity/{EndPoints.CheckProfanity}";

            // Act

            HttpResponseMessage response = await HelpCallControllerAsync(url, txtName, language);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            expectedContentType.Should().Be(response.Content.Headers.ContentType.ToString());
            response.Dispose();
        }

        [Test, Order(4)]
        [TestCase("TestData.txt", Language.EN)]
        public async Task Upload_SavesTextAndReturn_Success(string txtName, Language language)
        {
            // Arrange
            var url = $"/api/{version}/Profanity/{EndPoints.CheckProfanity}";

            // Act
            HttpResponseMessage response = await HelpCallControllerAsync(url, txtName, language);

            // Assert
            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();
        }

        [Test, Order(5)]
        public async Task Remove_WordsFrom_Profanity_List_Should_work()
        {
            var request = new RequestModel
            {
                Language = Language.EN,
                ProfanityWord = new List<string> { "fuck" }
            };

            var response = await _client.PostAsync($"/api/{version}/Profanity/{EndPoints.RemoveWordFromProfanity}", ConvertObjectToStringContent(request));
            response.EnsureSuccessStatusCode();
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();

        }

        [Test, Order(6)]
        public async Task Clear_Specific_Profanities_Should_Remove_All_Prfanities()
        {
            var response = await _client.DeleteAsync($"/api/{version}/Profanity/{EndPoints.ClearSpecificProfanitites}");
            response.EnsureSuccessStatusCode();
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Dispose();
        }

        [Test, Order(7)]
        public async Task Clear_All_Profanities_Should_Remove_All_Prfanities()
        {
            var result = await _client.DeleteAsync($"/api/{version}/Profanity/{EndPoints.ClearAllProfanities}");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        private async Task<HttpResponseMessage> HelpCallControllerAsync(string url, string fileName, Language language)
        {
            using (var file1 = File.OpenRead(GetPath(fileName)))
            using (var content = new StreamContent(file1))
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(content, "file", "testFile.txt");
                formData.Add(ConvertLanguageToByteArray(language), "language");
                return await _client.PostAsync(url, formData);
            }
        }

        private ByteArrayContent ConvertLanguageToByteArray<T>(T language)
        {
            var myContent = JsonConvert.SerializeObject(language);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            return new ByteArrayContent(buffer);
        }

        private StringContent ConvertObjectToStringContent<T>(T data)
        {
            string jsonString = System.Text.Json.JsonSerializer.Serialize(data);
            var stringContent = new StringContent(jsonString, UnicodeEncoding.UTF8, "application/json");
            return stringContent;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}