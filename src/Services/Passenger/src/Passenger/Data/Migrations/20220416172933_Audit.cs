using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Passenger.Data.Migrations
{
    public partial class Audit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                schema: "dbo",
                table: "Passenger",
                newName: "LastModifiedBy");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                schema: "dbo",
                table: "Passenger",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Passenger",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Passenger",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "dbo",
                table: "Passenger",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Passenger");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "Passenger");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "dbo",
                table: "Passenger");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                schema: "dbo",
                table: "Passenger",
                newName: "ModifiedBy");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                schema: "dbo",
                table: "Passenger",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
