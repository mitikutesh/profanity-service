using Microsoft.EntityFrameworkCore;
using Profanity.Data.Entities;
using System;

namespace Profanity.Data
{
    public class ProfanityServiceDbContext : DbContext, IProfanityServiceDbContext
    {
        public ProfanityServiceDbContext()
        {

        }
        public ProfanityServiceDbContext(DbContextOptions<ProfanityServiceDbContext> options)
            : base(options)
        {

        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new Exception("MortgagePlannerContext is not configured. Ensure ConnectionString is set.");
            }
        }


        public DbSet<ProfanityEntity> ProfanityEntities { get; set; }
        public virtual DbSet<User> Api_Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfanityEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Language)
                .HasConversion<int>();
                
            });

            modelBuilder.Entity<User>().OwnsMany(p => p.Refresh_Tokens, a =>
            {
                a.WithOwner().HasForeignKey("OwnerId");
                a.Property<int>("Id");
                a.HasKey("Id");
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

    public interface IProfanityServiceDbContext
    {
        DbSet<ProfanityEntity> ProfanityEntities { get; }
        int SaveChanges();
    }
}
