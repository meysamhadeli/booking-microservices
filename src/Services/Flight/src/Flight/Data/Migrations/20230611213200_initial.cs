using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flight.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aircraft",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    manufacturingyear = table.Column<int>(name: "manufacturing_year", type: "integer", maxLength: 5, nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: true),
                    createdby = table.Column<long>(name: "created_by", type: "bigint", nullable: true),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<long>(name: "last_modified_by", type: "bigint", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aircraft", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "airport",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: true),
                    createdby = table.Column<long>(name: "created_by", type: "bigint", nullable: true),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<long>(name: "last_modified_by", type: "bigint", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_airport", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "flight",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    flightnumber = table.Column<string>(name: "flight_number", type: "character varying(50)", maxLength: 50, nullable: false),
                    aircraftid = table.Column<Guid>(name: "aircraft_id", type: "uuid", nullable: false),
                    departureairportid = table.Column<Guid>(name: "departure_airport_id", type: "uuid", nullable: false),
                    arriveairportid = table.Column<Guid>(name: "arrive_airport_id", type: "uuid", nullable: false),
                    durationminutes = table.Column<decimal>(name: "duration_minutes", type: "numeric", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    price = table.Column<decimal>(type: "numeric", maxLength: 10, nullable: false),
                    arrivedate = table.Column<DateTime>(name: "arrive_date", type: "timestamp with time zone", nullable: false),
                    departuredate = table.Column<DateTime>(name: "departure_date", type: "timestamp with time zone", nullable: false),
                    flightdate = table.Column<DateTime>(name: "flight_date", type: "timestamp with time zone", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: true),
                    createdby = table.Column<long>(name: "created_by", type: "bigint", nullable: true),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<long>(name: "last_modified_by", type: "bigint", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flight", x => x.id);
                    table.ForeignKey(
                        name: "fk_flight_aircraft_aircraft_id",
                        column: x => x.aircraftid,
                        principalTable: "aircraft",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flight_airport_arrive_airport_id",
                        column: x => x.arriveairportid,
                        principalTable: "airport",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flight_airport_departure_airport_id",
                        column: x => x.departureairportid,
                        principalTable: "airport",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seatnumber = table.Column<string>(name: "seat_number", type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    @class = table.Column<string>(name: "class", type: "text", nullable: false, defaultValue: "Unknown"),
                    flightid = table.Column<Guid>(name: "flight_id", type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: true),
                    createdby = table.Column<long>(name: "created_by", type: "bigint", nullable: true),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<long>(name: "last_modified_by", type: "bigint", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seat", x => x.id);
                    table.ForeignKey(
                        name: "fk_seat_flight_flight_id",
                        column: x => x.flightid,
                        principalTable: "flight",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_flight_aircraft_id",
                table: "flight",
                column: "aircraft_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_arrive_airport_id",
                table: "flight",
                column: "arrive_airport_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_departure_airport_id",
                table: "flight",
                column: "departure_airport_id");

            migrationBuilder.CreateIndex(
                name: "ix_seat_flight_id",
                table: "seat",
                column: "flight_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seat");

            migrationBuilder.DropTable(
                name: "flight");

            migrationBuilder.DropTable(
                name: "aircraft");

            migrationBuilder.DropTable(
                name: "airport");
        }
    }
}
