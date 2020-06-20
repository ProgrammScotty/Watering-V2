using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Watering2.Migrations
{
    public partial class Watering2DbContextWateringContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    Humidity = table.Column<double>(nullable: false),
                    Pressure = table.Column<double>(nullable: false),
                    Raining = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Waterings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<double>(nullable: false),
                    CorrHot = table.Column<double>(nullable: false),
                    CorrCold = table.Column<double>(nullable: false),
                    EmergencyWatering = table.Column<bool>(nullable: false),
                    NoWateringBecauseRain = table.Column<bool>(nullable: false),
                    PercentageHot = table.Column<double>(nullable: false),
                    PercentageCold = table.Column<double>(nullable: false),
                    SamplesCount = table.Column<int>(nullable: false),
                    SamplesHot = table.Column<int>(nullable: false),
                    SamplesCold = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waterings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropTable(
                name: "Waterings");
        }
    }
}
