using NUnit.Framework;
using Profanity.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Profanity.Service.Test
{
    public class ProfanityServiceBaseTests
    {
        private List<string> profanitites = new List<string>() { "fuck" };
        private IProfanityWord _profanityWord;

        [SetUp]
        public void Setup()
        {
          //  profanityService = new ProfanityService(profanitites);
        }

    }
}