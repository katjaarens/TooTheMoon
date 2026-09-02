using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TooTheMoon.Migrations
{
    /// <inheritdoc />
    public partial class AddAdultsAndChildren : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdultsCount",
                table: "RsvpGuests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildrenCount",
                table: "RsvpGuests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChildrenDietaryNotes",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoodIntolerances",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageToCouple",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SongRequest",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdultsCount",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "ChildrenCount",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "ChildrenDietaryNotes",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "FoodIntolerances",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "MessageToCouple",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "SongRequest",
                table: "RsvpGuests");
        }
    }
}
