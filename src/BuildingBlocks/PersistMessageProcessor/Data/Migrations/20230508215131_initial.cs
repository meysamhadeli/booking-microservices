using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingBlocks.PersistMessageProcessor.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "persist_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    datatype = table.Column<string>(name: "data_type", type: "text", nullable: true),
                    data = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    retrycount = table.Column<int>(name: "retry_count", type: "integer", nullable: false),
                    messagestatus = table.Column<string>(name: "message_status", type: "text", nullable: false, defaultValue: "InProgress"),
                    deliverytype = table.Column<string>(name: "delivery_type", type: "text", nullable: false, defaultValue: "Outbox"),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_persist_message", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "persist_message");
        }
    }
}
