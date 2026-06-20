using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AnalyticsReporting_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_analytics",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    current_streak = table.Column<int>(type: "int", nullable: false),
                    longest_streak = table.Column<int>(type: "int", nullable: false),
                    last_log_date = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    weekly_completion_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    last_adherence_score = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    last_progress_calculated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_analytics", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "progress_snapshots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_analytics_id = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    total_calories = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_protein_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_carbs_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_fat_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    adherence_score = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    is_complete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_progress_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "f_k_progress_snapshots_user_analytics_user_analytics_id",
                        column: x => x.user_analytics_id,
                        principalTable: "user_analytics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_progress_snapshots_analytics_date",
                table: "progress_snapshots",
                columns: new[] { "user_analytics_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_user_analytics_records_user_id",
                table: "user_analytics",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "progress_snapshots");

            migrationBuilder.DropTable(
                name: "user_analytics");
        }
    }
}
