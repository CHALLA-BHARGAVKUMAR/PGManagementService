using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableColumn_Room : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableBeds",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableBeds",
                table: "Rooms");
        }
    }
}
