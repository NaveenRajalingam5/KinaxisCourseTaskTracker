using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinaxisCourseTaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToVerificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EmailVerificationTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EmailVerificationTokens");
        }
    }
}
