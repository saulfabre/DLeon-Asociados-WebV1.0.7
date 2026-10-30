using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSeguroClientDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClienteApellido",
                table: "Seguros",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClienteCorreo",
                table: "Seguros",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClienteDireccion",
                table: "Seguros",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClienteDocumento",
                table: "Seguros",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClienteNombre",
                table: "Seguros",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClienteTelefono",
                table: "Seguros",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClienteApellido",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ClienteCorreo",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ClienteDireccion",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ClienteDocumento",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ClienteNombre",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ClienteTelefono",
                table: "Seguros");
        }
    }
}
