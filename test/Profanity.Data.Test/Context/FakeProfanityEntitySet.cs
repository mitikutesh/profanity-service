using Profanity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profanity.Data.Test.Context
{
    public class FakeProfanityEntitySet : FakeDbSet<ProfanityEntity>
    {
        public override ProfanityEntity Find(params object[] keyValues)
        {
            return this.SingleOrDefault(d => d.Id == (Guid)keyValues.Single());
        }
    }
}
