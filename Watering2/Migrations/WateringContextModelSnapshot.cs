﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Watering2.DbContext;

namespace Watering2.Migrations
{
    [DbContext(typeof(WateringContext))]
    partial class WateringContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("Watering2.Models.Error", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Errors");
                });

            modelBuilder.Entity("Watering2.Models.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("DewPoint")
                        .HasColumnType("REAL");

                    b.Property<double>("Humidity")
                        .HasColumnType("REAL");

                    b.Property<double>("Pressure")
                        .HasColumnType("REAL");

                    b.Property<bool>("Raining")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Temperature")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Measurements");
                });

            modelBuilder.Entity("Watering2.Models.WateringData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("CorrCold")
                        .HasColumnType("REAL");

                    b.Property<double>("CorrHot")
                        .HasColumnType("REAL");

                    b.Property<double>("Duration")
                        .HasColumnType("REAL");

                    b.Property<double>("DurationRain")
                        .HasColumnType("REAL");

                    b.Property<bool>("EmergencyWatering")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("NoWateringBecauseRain")
                        .HasColumnType("INTEGER");

                    b.Property<double>("PercentageCold")
                        .HasColumnType("REAL");

                    b.Property<double>("PercentageHot")
                        .HasColumnType("REAL");

                    b.Property<int>("SamplesCold")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SamplesCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SamplesHot")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Waterings");
                });
#pragma warning restore 612, 618
        }
    }
}
