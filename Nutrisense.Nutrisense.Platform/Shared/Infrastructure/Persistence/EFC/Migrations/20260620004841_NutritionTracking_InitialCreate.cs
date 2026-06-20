using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class NutritionTracking_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "foods",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    source = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    external_id = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    name_en = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    name_es = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Other"),
                    serving_size_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    serving_unit = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    calories_per100g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    protein_per100g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    carbs_per100g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    fat_per100g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    fiber_per100g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    sugar_per100g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    restrictions = table.Column<string>(type: "json", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_foods", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "nutrition_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    food_id = table.Column<int>(type: "int", nullable: false),
                    food_name_en = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    food_name_es = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    meal_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    date = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    quantity_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    calories = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    protein_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    carbs_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    fat_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    fiber_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    sugar_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    source = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    logged_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    scan_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    scan_confidence = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    scan_image_uri = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_nutrition_logs", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_foods_external_id",
                table: "foods",
                column: "external_id");

            migrationBuilder.CreateIndex(
                name: "ix_foods_key",
                table: "foods",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_nutrition_logs_user_date",
                table: "nutrition_logs",
                columns: new[] { "user_id", "date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "foods");

            migrationBuilder.DropTable(
                name: "nutrition_logs");
        }
    }
}
