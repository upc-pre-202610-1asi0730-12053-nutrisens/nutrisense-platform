using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class ActivityWearable_HealthSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "access_token",
                table: "wearable_connections",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "auto_sync_enabled",
                table: "wearable_connections",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "wearable_connections",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "token_expires_at",
                table: "wearable_connections",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "access_token",
                table: "wearable_connections");

            migrationBuilder.DropColumn(
                name: "auto_sync_enabled",
                table: "wearable_connections");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "wearable_connections");

            migrationBuilder.DropColumn(
                name: "token_expires_at",
                table: "wearable_connections");
        }
    }
}
