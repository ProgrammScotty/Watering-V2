using Microsoft.EntityFrameworkCore.Migrations;

namespace Watering2.Migrations
{
    public partial class DurationRain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DurationRain",
                table: "Waterings",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationRain",
                table: "Waterings");
        }
    }
}
