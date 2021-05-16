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
                entity.Property(e => e.Language)
                .HasConversion<int>();
                
            });
            base.OnModelCreating(modelBuilder);
        }

        private static void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
        }
    }

    public static class EntityExtensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
