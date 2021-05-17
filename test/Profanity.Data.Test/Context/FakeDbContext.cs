using NUnit.Framework;
using Profanity.Data.DTO;
using Profanity.Data.Entities;
using Profanity.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace Profanity.Data.Test.Context
{
    
    public class FakeDbContext  
    {
        public FakeDbContext()
        {
           // ProfanityEntities = new FakeProfanityEntitySet();
        }

      //  public Microsoft.EntityFrameworkCore.DbSet<ProfanityEntity> ProfanityEntities => new FakeProfanityEntitySet();


        //public DbSet<ProfanityEntity> ProfanityEntities { get; private set; }

        public int SaveChanges()
        {
            return 0;
        }

    }
}
