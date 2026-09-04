using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TooTheMoon.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatingCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PosX",
                table: "WeddingTables",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PosY",
                table: "WeddingTables",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosX",
                table: "WeddingTables");

            migrationBuilder.DropColumn(
                name: "PosY",
                table: "WeddingTables");
        }
    }
}
