using Microsoft.EntityFrameworkCore;
using Profanity.Data.Entities;

namespace Profanity.Data
{
    public class ProfanityServiceDbContext : DbContext
    {
        public ProfanityServiceDbContext()
        {

        }
        public ProfanityServiceDbContext(DbContextOptions<ProfanityServiceDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<ProfanityEntity> ProfanityEntities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfanityEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            base.OnModelCreating(modelBuilder);
        }

        private static void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
        }
    }
}
