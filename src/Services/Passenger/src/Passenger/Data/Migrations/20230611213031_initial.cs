using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passenger.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "passenger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    passportnumber = table.Column<string>(name: "passport_number", type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    passengertype = table.Column<string>(name: "passenger_type", type: "text", nullable: false, defaultValue: "Unknown"),
                    age = table.Column<int>(type: "integer", maxLength: 3, nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: true),
                    createdby = table.Column<long>(name: "created_by", type: "bigint", nullable: true),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<long>(name: "last_modified_by", type: "bigint", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_passenger", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "passenger");
        }
    }
}
