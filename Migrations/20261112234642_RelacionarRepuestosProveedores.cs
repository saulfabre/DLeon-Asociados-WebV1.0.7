using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class RelacionarRepuestosProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Repuestos",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE repuesto
                SET repuesto.ProveedorId = proveedor.Id
                FROM [Repuestos] AS repuesto
                INNER JOIN (
                    SELECT Nombre, MIN(Id) AS Id
                    FROM [Proveedores]
                    WHERE [Existe] = 1
                    GROUP BY Nombre
                    HAVING COUNT(*) = 1
                ) AS proveedor ON proveedor.Nombre = repuesto.Proveedor;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_ProveedorId",
                table: "Repuestos",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repuestos_Proveedores_ProveedorId",
                table: "Repuestos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repuestos_Proveedores_ProveedorId",
                table: "Repuestos");

            migrationBuilder.DropIndex(
                name: "IX_Repuestos_ProveedorId",
                table: "Repuestos");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Repuestos");
        }
    }
}
