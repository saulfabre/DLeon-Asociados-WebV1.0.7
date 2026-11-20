using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDescripcionProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Proveedores",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Proveedores");
        }
    }
}
