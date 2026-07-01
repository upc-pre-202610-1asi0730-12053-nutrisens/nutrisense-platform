using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class Subscriptions_StripeCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "stripe_customer_id",
                table: "user_subscriptions",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stripe_customer_id",
                table: "user_subscriptions");
        }
    }
}
