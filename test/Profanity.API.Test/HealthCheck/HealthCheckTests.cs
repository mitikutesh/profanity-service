using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using Profanity.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Test.HealthCheck
{
    public class APIWebApplicationFactory : WebApplicationFactory<Startup>
    {
    }
    [TestFixture]
    public class HealthCheckTests
    {
        private APIWebApplicationFactory _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void GivenARequestToTheHealthCheck()
        {
            _factory = new APIWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetOverall_HealthCheck_Endpoint()
        {
           
            var req = new HttpRequestMessage(HttpMethod.Get, $"api/{EndPoints.HealthQuickCheck}");
            var resp = await _client.SendAsync(req);
            Assert.AreEqual(resp.IsSuccessStatusCode, true);
        }
        [Test]
        public async Task GetService_HealthCheck_Endpoint()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"api/{EndPoints.HealthService}");
            var resp = await _client.SendAsync(req);
            Assert.AreEqual(resp.IsSuccessStatusCode, true);
        }

        [Test]
        public async Task GetDatabase_HealthCheck_Endpoint()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"api/{EndPoints.HealthDatabase}");
            var resp = await _client.SendAsync(req);
            Assert.AreEqual(resp.IsSuccessStatusCode, true);
        }
    }
}
