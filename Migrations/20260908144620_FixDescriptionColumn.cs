using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TooTheMoon.Migrations
{
    /// <inheritdoc />
    public partial class FixDescriptionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groomsmaids",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    imagepath = table.Column<string>(type: "text", nullable: true),
                    rolebadge = table.Column<string>(type: "text", nullable: true),
                    anecdote = table.Column<string>(type: "text", nullable: true),
                    firstimpression = table.Column<string>(type: "text", nullable: true),
                    sincewhen = table.Column<string>(type: "text", nullable: true),
                    speciality = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groomsmaids", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "groomsmaids");
        }
    }
}
