using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flight.Data.Migrations
{
    public partial class AddVersening : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "dbo",
                table: "Seat",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "dbo",
                table: "Flight",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "dbo",
                table: "Airport",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "dbo",
                table: "Aircraft",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                schema: "dbo",
                table: "Seat");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "dbo",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "dbo",
                table: "Airport");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "dbo",
                table: "Aircraft");
        }
    }
}
