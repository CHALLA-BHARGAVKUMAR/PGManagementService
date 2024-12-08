using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGManagementService.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAppliationUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "OTP");

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiration",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPExpiration",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OTP",
                table: "AspNetUsers",
                newName: "Name");
        }
    }
}
