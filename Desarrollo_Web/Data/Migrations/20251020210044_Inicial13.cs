using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemonSlayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aceptada",
                table: "Postulaciones");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Postulaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Postulaciones");

            migrationBuilder.AddColumn<bool>(
                name: "Aceptada",
                table: "Postulaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
