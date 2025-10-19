using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemonSlayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinatarioId",
                table: "Mensajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Leido",
                table: "Mensajes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinatarioId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "Leido",
                table: "Mensajes");
        }
    }
}
