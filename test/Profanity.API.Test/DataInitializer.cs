using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.API.Test
{
    public class DataInitializer
    {
        private static byte[] EncodeHelper(List<string> ls)
        {
            var str = string.Join(',', ls);
            return Encoding.UTF8.GetBytes(str);
        }

        public static List<ProfanityEntity> GetProfanityEntities()
        {
            var profanityEntities = new List<ProfanityEntity> {
                    new ProfanityEntity() {
                    Id =new Guid("D9CD8966-7332-452E-93EF-B7E942CE5837"),
                    Language = Language.FI,
                    ProfanityWord = EncodeHelper(new List<string>{ "fuck", "ass"})
                    },
                    new ProfanityEntity() {
                    Id =new Guid("D9CD8966-7332-452E-93EF-B7E942CE5837"),
                    Language = Language.FI,
                    ProfanityWord = EncodeHelper(new List<string>{ "vitu" })
                },
              };
            return profanityEntities;
        }
    }
}