using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Watering2.Models;

namespace Watering2.DbContext
{
    public partial class WateringContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public WateringContext()
        {
        }

        public WateringContext(DbContextOptions<WateringContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Error> Errors { get; set; }
        public virtual DbSet<Measurement> Measurements { get; set; }
        public virtual DbSet<WateringData> Waterings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=Watering.sqlite;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
