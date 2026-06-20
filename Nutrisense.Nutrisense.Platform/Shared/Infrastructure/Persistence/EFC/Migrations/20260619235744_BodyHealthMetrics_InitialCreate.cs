using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class BodyHealthMetrics_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "body_metrics",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    height_cm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    date_of_birth = table.Column<string>(type: "longtext", nullable: true),
                    biological_sex = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    activity_level = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    bmi_value = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    bmi_category = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    bmr = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    tdee = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    macro_daily_calories = table.Column<int>(type: "int", nullable: true),
                    macro_protein_g = table.Column<decimal>(type: "decimal(6,1)", nullable: true),
                    macro_carbs_g = table.Column<decimal>(type: "decimal(6,1)", nullable: true),
                    macro_fat_g = table.Column<decimal>(type: "decimal(6,1)", nullable: true),
                    macro_fiber_g = table.Column<decimal>(type: "decimal(6,1)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_body_metrics", x => x.id);
                    table.UniqueConstraint("a_k_body_metrics_user_id", x => x.user_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "body_measurements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    waist_cm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    neck_cm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    measured_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_body_measurements", x => x.id);
                    table.ForeignKey(
                        name: "f_k_body_measurements_body_metrics_user_id",
                        column: x => x.user_id,
                        principalTable: "body_metrics",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_goals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    goal = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    start_weight_kg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    target_weight_kg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    weekly_rate_kg = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    daily_calorie_target = table.Column<int>(type: "int", nullable: false),
                    protein_target_g = table.Column<decimal>(type: "decimal(6,1)", nullable: false),
                    carbs_target_g = table.Column<decimal>(type: "decimal(6,1)", nullable: false),
                    fat_target_g = table.Column<decimal>(type: "decimal(6,1)", nullable: false),
                    fiber_target_g = table.Column<decimal>(type: "decimal(6,1)", nullable: false),
                    caloric_adjustment = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    set_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_goals", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_goals_body_metrics_user_id",
                        column: x => x.user_id,
                        principalTable: "body_metrics",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "weight_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    weight_kg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    logged_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    note = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_weight_logs", x => x.id);
                    table.ForeignKey(
                        name: "f_k_weight_logs_body_metrics_user_id",
                        column: x => x.user_id,
                        principalTable: "body_metrics",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_body_measurements_user_id",
                table: "body_measurements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_user_goals_user_id",
                table: "user_goals",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_weight_logs_user_id",
                table: "weight_logs",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "body_measurements");

            migrationBuilder.DropTable(
                name: "user_goals");

            migrationBuilder.DropTable(
                name: "weight_logs");

            migrationBuilder.DropTable(
                name: "body_metrics");
        }
    }
}
