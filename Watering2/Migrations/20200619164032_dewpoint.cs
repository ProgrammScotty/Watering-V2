using Microsoft.EntityFrameworkCore.Migrations;

namespace Watering2.Migrations
{
    public partial class dewpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DewPoint",
                table: "Measurements",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DewPoint",
                table: "Measurements");
        }
    }
}
