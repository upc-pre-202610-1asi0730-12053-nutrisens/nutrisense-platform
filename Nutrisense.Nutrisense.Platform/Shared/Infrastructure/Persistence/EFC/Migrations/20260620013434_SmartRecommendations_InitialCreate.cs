using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class SmartRecommendations_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name_en = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_es = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    lat = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    lng = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    timezone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_cities", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ingredient_catalog_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    name_es = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    food_id = table.Column<int>(type: "int", nullable: true),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    default_unit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    grams_per_unit = table.Column<decimal>(type: "decimal(10,3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ingredient_catalog_items", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_preferences",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    home_city_id = table.Column<int>(type: "int", nullable: true),
                    current_city_id = table.Column<int>(type: "int", nullable: true),
                    travel_mode_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_manual_override = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    location_permission_granted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_location_preferences", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pantries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_pantries", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    name_es = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    goal_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    prep_time_minutes = table.Column<int>(type: "int", nullable: false),
                    servings = table.Column<int>(type: "int", nullable: false),
                    total_calories = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_protein_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_carbs_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_fat_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_fiber_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    restrictions_conflict = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_recipes", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recommendation_cards",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    city_id = table.Column<int>(type: "int", nullable: true),
                    weather_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    goal_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    food_id = table.Column<int>(type: "int", nullable: true),
                    food_name_en = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    food_name_es = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    estimated_calories = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    estimated_protein_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    estimated_carbs_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    estimated_fat_g = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    badge = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    context_label_en = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    context_label_es = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    restrictions_conflict = table.Column<string>(type: "json", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_recommendation_cards", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pantry_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    pantry_id = table.Column<int>(type: "int", nullable: false),
                    ingredient_catalog_item_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    unit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_pantry_items", x => x.id);
                    table.ForeignKey(
                        name: "f_k_pantry_items_pantries_pantry_id",
                        column: x => x.pantry_id,
                        principalTable: "pantries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recipe_ingredient_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    ingredient_catalog_item_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    unit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_recipe_ingredient_items", x => x.id);
                    table.ForeignKey(
                        name: "f_k_recipe_ingredient_items_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_cities_key",
                table: "cities",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ingredient_catalog_items_key",
                table: "ingredient_catalog_items",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_location_preferences_user_id",
                table: "location_preferences",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pantries_user_id",
                table: "pantries",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_pantry_items_pantry_id",
                table: "pantry_items",
                column: "pantry_id");

            migrationBuilder.CreateIndex(
                name: "i_x_recipe_ingredient_items_recipe_id",
                table: "recipe_ingredient_items",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_key",
                table: "recipes",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recommendation_cards_user_id",
                table: "recommendation_cards",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "ingredient_catalog_items");

            migrationBuilder.DropTable(
                name: "location_preferences");

            migrationBuilder.DropTable(
                name: "pantry_items");

            migrationBuilder.DropTable(
                name: "recipe_ingredient_items");

            migrationBuilder.DropTable(
                name: "recommendation_cards");

            migrationBuilder.DropTable(
                name: "pantries");

            migrationBuilder.DropTable(
                name: "recipes");
        }
    }
}
