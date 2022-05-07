using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Booking",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Trip_FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Trip_AircraftId = table.Column<long>(type: "bigint", nullable: true),
                    Trip_DepartureAirportId = table.Column<long>(type: "bigint", nullable: true),
                    Trip_ArriveAirportId = table.Column<long>(type: "bigint", nullable: true),
                    Trip_FlightDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Trip_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Trip_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Trip_SeatNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassengerInfo_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking",
                schema: "dbo");
        }
    }
}
