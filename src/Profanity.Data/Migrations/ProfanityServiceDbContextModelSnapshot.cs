﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Profanity.Data;

namespace Profanity.Data.Migrations
{
    [DbContext(typeof(ProfanityServiceDbContext))]
    partial class ProfanityServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("Profanity.Data.Entities.ProfanityEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfanityWord")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ProfanityEntities");
                });
#pragma warning restore 612, 618
        }
    }
}
