using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TooTheMoon.Migrations
{
    /// <inheritdoc />
    public partial class AddMeatCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildrenDietaryNotes",
                table: "RsvpGuests");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "MeatCount",
                table: "RsvpGuests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VeganCount",
                table: "RsvpGuests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VeggieCount",
                table: "RsvpGuests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeatCount",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "VeganCount",
                table: "RsvpGuests");

            migrationBuilder.DropColumn(
                name: "VeggieCount",
                table: "RsvpGuests");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChildrenDietaryNotes",
                table: "RsvpGuests",
                type: "TEXT",
                nullable: true);
        }
    }
}
