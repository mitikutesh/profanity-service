using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Profanity.API.Controllers;
using Profanity.API.Helper;
using Profanity.API.Model;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Service;
using Profanity.Service.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Test
{
    //public class APIWebApplicationFactory : WebApplicationFactory<Startup>
    //{
    //}
    [TestFixture]
    public class ProfanityControllerTests
    {
     


        private IProfanityService profanityService;
        private ProfanityController _ctr;

        [SetUp]
        public void Setup()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapping()); 
            });
            var mapper = mockMapper.CreateMapper();
            profanityService = new ProfanityService(new FakeProfanityDb());

            _ctr = new ProfanityController(profanityService, mapper);
        }

        private string version = "v1";


        private static string GetPath(string fileName)
            => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

        [Test, Order(1)]
        public async Task Add_Profanity_word_Tolist_Should_work()
        {
            var request = new RequestModel
            {
                Language = Language.EN,
                ProfanityWord = new List<string> { "test" }
            };
            var result = await _ctr.PostAddToProfanityList(request);
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test, Order(2)]
        [TestCase(Language.EN)]
        public async Task Get_Profanity_List_By_Language_Should_Retrieve_Profanities(Language language)
        {
            var result = await _ctr.Get(language);
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }


        [Test, Order(3)]
        [TestCase("TestData.txt", Language.EN)]
        public async Task Upload_SavesTextAndReturn_Success(string txtName, Language language)
        {
            var physicalFile = new FileInfo(GetPath(txtName));

            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            using (FileStream fs = physicalFile.OpenRead())
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    writer.WriteLine(temp.GetString(b));
                }
            }
            writer.Flush();
            ms.Position = 0;
            var fileName = physicalFile.Name;
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(m => m.OpenReadStream()).Returns(ms);
            fileMock.Setup(m => m.ContentDisposition).Returns(string.Format("inline; file={0}", fileName));
            var file =  fileMock.Object;


            var result = await _ctr.Post(file, Language.FI);
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test, Order(4)]
        public async Task Remove_WordsFrom_Profanity_List_Should_work()
        {
            var request = new RequestModel
            {
                Language = Language.EN,
                ProfanityWord = new List<string> { "test" }
            };
            var result = await _ctr.PostRemoveFromProfanityList(request);
            result.Result.Should().BeOfType(typeof(OkObjectResult));

        }

        [Test, Order(5)]
        [TestCase(Language.EN)]
        public async Task Clear_Specific_Profanities_Should_Remove_All_Prfanities(Language language)
        {
           
            var result = await _ctr.DeleteByLanguage(language);
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test, Order(7)]
        public async Task Clear_All_Profanities_Should_Remove_All_Prfanities()
        {
            var result = await _ctr.Delete();
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

    }
}