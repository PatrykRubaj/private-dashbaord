using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSleepEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sleep_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sleep_hours = table.Column<decimal>(type: "numeric", nullable: false),
                    recharge_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    credit_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    debt_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    balance_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    sleep_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    rem_hours = table.Column<decimal>(type: "numeric", nullable: true),
                    rem_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    deep_hours = table.Column<decimal>(type: "numeric", nullable: true),
                    deep_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    bpm = table.Column<int>(type: "integer", nullable: true),
                    bpm_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    sleep_rating = table.Column<decimal>(type: "numeric", nullable: true),
                    owner_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sleep_entries", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sleep_entries");
        }
    }
}
