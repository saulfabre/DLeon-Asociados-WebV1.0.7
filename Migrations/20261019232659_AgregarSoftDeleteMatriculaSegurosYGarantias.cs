using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSoftDeleteMatriculaSegurosYGarantias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes");

            migrationBuilder.AddColumn<string>(
                name: "Matricula",
                table: "Vehiculos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Existe",
                table: "Seguros",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroSeguro",
                table: "Seguros",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProveedorNombre",
                table: "Seguros",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoProveedor",
                table: "Seguros",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Propio");

            migrationBuilder.AddColumn<bool>(
                name: "Existe",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "Garantias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Existe = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "date", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garantias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garantias_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes",
                column: "Documento",
                unique: true,
                filter: "[Documento] IS NOT NULL AND [Existe] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Garantias_FechaExpiracion",
                table: "Garantias",
                column: "FechaExpiracion");

            migrationBuilder.CreateIndex(
                name: "IX_Garantias_VehiculoId",
                table: "Garantias",
                column: "VehiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Garantias");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Matricula",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Existe",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "NumeroSeguro",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ProveedorNombre",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "TipoProveedor",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "Existe",
                table: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes",
                column: "Documento",
                unique: true,
                filter: "[Documento] IS NOT NULL");
        }
    }
}
