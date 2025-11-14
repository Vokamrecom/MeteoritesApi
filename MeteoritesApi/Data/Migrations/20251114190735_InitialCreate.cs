using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeteoritesApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ingestion_runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    hangfire_job_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    inserted = table.Column<int>(type: "integer", nullable: false),
                    updated = table.Column<int>(type: "integer", nullable: false),
                    deleted = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingestion_runs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "meteorites",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    recclass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mass_gram = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    year = table.Column<int>(type: "integer", nullable: true),
                    fall = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    geolocation_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meteorites", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_runs_started_at",
                table: "ingestion_runs",
                column: "started_at");

            migrationBuilder.CreateIndex(
                name: "ix_meteorites_recclass",
                table: "meteorites",
                column: "recclass");

            migrationBuilder.CreateIndex(
                name: "ix_meteorites_year",
                table: "meteorites",
                column: "year");

            migrationBuilder.CreateIndex(
                name: "ix_meteorites_year_recclass",
                table: "meteorites",
                columns: new[] { "year", "recclass" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ingestion_runs");

            migrationBuilder.DropTable(
                name: "meteorites");
        }
    }
}
