using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionProveedoresRepuestos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProveedoresRepuestos",
                columns: table => new
                {
                    DetalleProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    RepuestoId = table.Column<int>(type: "int", nullable: false),
                    PrecioCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CodigoReferencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TiempoEntrega = table.Column<int>(type: "int", nullable: true),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedoresRepuestos", x => x.DetalleProveedorId);
                    table.ForeignKey(
                        name: "FK_ProveedoresRepuestos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProveedoresRepuestos_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProveedoresRepuestos_ProveedorId_RepuestoId",
                table: "ProveedoresRepuestos",
                columns: new[] { "ProveedorId", "RepuestoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedoresRepuestos_RepuestoId",
                table: "ProveedoresRepuestos",
                column: "RepuestoId");

            migrationBuilder.Sql("""
                INSERT INTO [ProveedoresRepuestos] ([ProveedorId], [RepuestoId], [EsPrincipal], [Estado])
                SELECT [ProveedorId], [Id], 1, 1
                FROM [Repuestos]
                WHERE [ProveedorId] IS NOT NULL
                  AND EXISTS (SELECT 1 FROM [Proveedores] p WHERE p.[Id] = [Repuestos].[ProveedorId]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProveedoresRepuestos");
        }
    }
}
