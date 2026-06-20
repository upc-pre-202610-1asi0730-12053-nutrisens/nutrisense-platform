using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class Subscriptions_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    last_four = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    brand = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    expiry_month = table.Column<int>(type: "int", nullable: false),
                    expiry_year = table.Column<int>(type: "int", nullable: false),
                    cardholder_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    stripe_payment_method_id = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_payment_methods", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    price_monthly = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    price_annual = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    features = table.Column<string>(type: "json", nullable: false),
                    currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscription_plans", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    plan_key = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    billing_period = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    cancel_at_period_end = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    stripe_subscription_id = table.Column<string>(type: "longtext", nullable: true),
                    last_plan_change_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    payment_method_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_subscriptions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payment_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_subscription_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    stripe_payment_intent_id = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_payment_records", x => x.id);
                    table.ForeignKey(
                        name: "f_k_payment_records_user_subscriptions_user_subscription_id",
                        column: x => x.user_subscription_id,
                        principalTable: "user_subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_payment_records_user_subscription_id",
                table: "payment_records",
                column: "user_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plans_key",
                table: "subscription_plans",
                column: "key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "payment_records");

            migrationBuilder.DropTable(
                name: "subscription_plans");

            migrationBuilder.DropTable(
                name: "user_subscriptions");
        }
    }
}
