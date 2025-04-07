using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingMonolith.Flight.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "flight");

            migrationBuilder.CreateTable(
                name: "aircraft",
                schema: "flight",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    manufacturing_year = table.Column<int>(type: "integer", maxLength: 5, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aircraft", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "airport",
                schema: "flight",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_airport", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "flight",
                schema: "flight",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    flight_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aircraft_id = table.Column<Guid>(type: "uuid", nullable: false),
                    departure_airport_id = table.Column<Guid>(type: "uuid", nullable: false),
                    arrive_airport_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_minutes = table.Column<decimal>(type: "numeric", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    price = table.Column<decimal>(type: "numeric", maxLength: 10, nullable: false),
                    arrive_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    departure_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    flight_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flight", x => x.id);
                    table.ForeignKey(
                        name: "fk_flight_aircraft_aircraft_id",
                        column: x => x.aircraft_id,
                        principalSchema: "flight",
                        principalTable: "aircraft",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flight_airport_arrive_airport_id",
                        column: x => x.arrive_airport_id,
                        principalSchema: "flight",
                        principalTable: "airport",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_flight_airport_departure_airport_id",
                        column: x => x.departure_airport_id,
                        principalSchema: "flight",
                        principalTable: "airport",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seat",
                schema: "flight",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seat_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "text", nullable: false, defaultValue: "Unknown"),
                    @class = table.Column<string>(name: "class", type: "text", nullable: false, defaultValue: "Unknown"),
                    flight_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seat", x => x.id);
                    table.ForeignKey(
                        name: "fk_seat_flight_flight_id",
                        column: x => x.flight_id,
                        principalSchema: "flight",
                        principalTable: "flight",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_flight_aircraft_id",
                schema: "flight",
                table: "flight",
                column: "aircraft_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_arrive_airport_id",
                schema: "flight",
                table: "flight",
                column: "arrive_airport_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_departure_airport_id",
                schema: "flight",
                table: "flight",
                column: "departure_airport_id");

            migrationBuilder.CreateIndex(
                name: "ix_seat_flight_id",
                schema: "flight",
                table: "seat",
                column: "flight_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seat",
                schema: "flight");

            migrationBuilder.DropTable(
                name: "flight",
                schema: "flight");

            migrationBuilder.DropTable(
                name: "aircraft",
                schema: "flight");

            migrationBuilder.DropTable(
                name: "airport",
                schema: "flight");
        }
    }
}
