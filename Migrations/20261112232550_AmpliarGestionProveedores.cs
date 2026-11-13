using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations;

public partial class AmpliarGestionProveedores : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "TipoProveedor",
            table: "Proveedores",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "NombreContacto",
            table: "Proveedores",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "PaginaWeb",
            table: "Proveedores",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Pais",
            table: "Proveedores",
            type: "nvarchar(80)",
            maxLength: 80,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Provincia",
            table: "Proveedores",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "TerminosPago",
            table: "Proveedores",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "Contado");

        migrationBuilder.AddColumn<decimal>(
            name: "LimiteCredito",
            table: "Proveedores",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<int>(
            name: "TiempoEntregaDias",
            table: "Proveedores",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "GarantiaOfrecida",
            table: "Proveedores",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "PoliticaDevolucion",
            table: "Proveedores",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Observaciones",
            table: "Proveedores",
            type: "nvarchar(2000)",
            maxLength: 2000,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "TipoVehiculos",
            table: "Proveedores",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "PaisProcedencia",
            table: "Proveedores",
            type: "nvarchar(80)",
            maxLength: 80,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "TipoPiezas",
            table: "Proveedores",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "TipoServicios",
            table: "Proveedores",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Especialidad",
            table: "Proveedores",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ZonaCobertura",
            table: "Proveedores",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "CategoriasPiezasProveedor",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CategoriasPiezasProveedor", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MarcasProveedor",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_MarcasProveedor", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ModelosVehiculosProveedor",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ModelosVehiculosProveedor", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ProveedoresCategoriasPiezas",
            columns: table => new
            {
                ProveedorId = table.Column<int>(type: "int", nullable: false),
                CategoriaPiezaId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProveedoresCategoriasPiezas", x => new { x.ProveedorId, x.CategoriaPiezaId });
                table.ForeignKey(
                    name: "FK_ProveedoresCategoriasPiezas_CategoriasPiezasProveedor_CategoriaPiezaId",
                    column: x => x.CategoriaPiezaId,
                    principalTable: "CategoriasPiezasProveedor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ProveedoresCategoriasPiezas_Proveedores_ProveedorId",
                    column: x => x.ProveedorId,
                    principalTable: "Proveedores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProveedoresMarcas",
            columns: table => new
            {
                ProveedorId = table.Column<int>(type: "int", nullable: false),
                MarcaId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProveedoresMarcas", x => new { x.ProveedorId, x.MarcaId });
                table.ForeignKey(
                    name: "FK_ProveedoresMarcas_MarcasProveedor_MarcaId",
                    column: x => x.MarcaId,
                    principalTable: "MarcasProveedor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ProveedoresMarcas_Proveedores_ProveedorId",
                    column: x => x.ProveedorId,
                    principalTable: "Proveedores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProveedoresModelosVehiculos",
            columns: table => new
            {
                ProveedorId = table.Column<int>(type: "int", nullable: false),
                ModeloVehiculoId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProveedoresModelosVehiculos", x => new { x.ProveedorId, x.ModeloVehiculoId });
                table.ForeignKey(
                    name: "FK_ProveedoresModelosVehiculos_ModelosVehiculosProveedor_ModeloVehiculoId",
                    column: x => x.ModeloVehiculoId,
                    principalTable: "ModelosVehiculosProveedor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ProveedoresModelosVehiculos_Proveedores_ProveedorId",
                    column: x => x.ProveedorId,
                    principalTable: "Proveedores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CategoriasPiezasProveedor_Nombre",
            table: "CategoriasPiezasProveedor",
            column: "Nombre",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MarcasProveedor_Nombre",
            table: "MarcasProveedor",
            column: "Nombre",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ModelosVehiculosProveedor_Nombre",
            table: "ModelosVehiculosProveedor",
            column: "Nombre",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ProveedoresCategoriasPiezas_CategoriaPiezaId",
            table: "ProveedoresCategoriasPiezas",
            column: "CategoriaPiezaId");

        migrationBuilder.CreateIndex(
            name: "IX_ProveedoresMarcas_MarcaId",
            table: "ProveedoresMarcas",
            column: "MarcaId");

        migrationBuilder.CreateIndex(
            name: "IX_ProveedoresModelosVehiculos_ModeloVehiculoId",
            table: "ProveedoresModelosVehiculos",
            column: "ModeloVehiculoId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ProveedoresCategoriasPiezas");
        migrationBuilder.DropTable(name: "ProveedoresMarcas");
        migrationBuilder.DropTable(name: "ProveedoresModelosVehiculos");
        migrationBuilder.DropTable(name: "CategoriasPiezasProveedor");
        migrationBuilder.DropTable(name: "MarcasProveedor");
        migrationBuilder.DropTable(name: "ModelosVehiculosProveedor");

        migrationBuilder.DropColumn(name: "Especialidad", table: "Proveedores");
        migrationBuilder.DropColumn(name: "GarantiaOfrecida", table: "Proveedores");
        migrationBuilder.DropColumn(name: "LimiteCredito", table: "Proveedores");
        migrationBuilder.DropColumn(name: "NombreContacto", table: "Proveedores");
        migrationBuilder.DropColumn(name: "Observaciones", table: "Proveedores");
        migrationBuilder.DropColumn(name: "PaginaWeb", table: "Proveedores");
        migrationBuilder.DropColumn(name: "Pais", table: "Proveedores");
        migrationBuilder.DropColumn(name: "PaisProcedencia", table: "Proveedores");
        migrationBuilder.DropColumn(name: "PoliticaDevolucion", table: "Proveedores");
        migrationBuilder.DropColumn(name: "Provincia", table: "Proveedores");
        migrationBuilder.DropColumn(name: "TerminosPago", table: "Proveedores");
        migrationBuilder.DropColumn(name: "TiempoEntregaDias", table: "Proveedores");
        migrationBuilder.DropColumn(name: "TipoPiezas", table: "Proveedores");
        migrationBuilder.DropColumn(name: "TipoProveedor", table: "Proveedores");
        migrationBuilder.DropColumn(name: "TipoServicios", table: "Proveedores");
        migrationBuilder.DropColumn(name: "TipoVehiculos", table: "Proveedores");
        migrationBuilder.DropColumn(name: "ZonaCobertura", table: "Proveedores");
    }
}
