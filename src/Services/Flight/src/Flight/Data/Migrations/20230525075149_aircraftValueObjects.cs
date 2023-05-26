using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flight.Data.Migrations
{
    /// <inheritdoc />
    public partial class aircraftValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_flight_aircraft_aircraft_id",
                table: "flight");

            migrationBuilder.AlterColumn<Guid>(
                name: "aircraft_id",
                table: "flight",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "aircraft",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "model",
                table: "aircraft",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "manufacturing_year",
                table: "aircraft",
                type: "integer",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_flight_aircraft_aircraft_id",
                table: "flight",
                column: "aircraft_id",
                principalTable: "aircraft",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_flight_aircraft_aircraft_id",
                table: "flight");

            migrationBuilder.AlterColumn<Guid>(
                name: "aircraft_id",
                table: "flight",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "aircraft",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "model",
                table: "aircraft",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "manufacturing_year",
                table: "aircraft",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_flight_aircraft_aircraft_id",
                table: "flight",
                column: "aircraft_id",
                principalTable: "aircraft",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
