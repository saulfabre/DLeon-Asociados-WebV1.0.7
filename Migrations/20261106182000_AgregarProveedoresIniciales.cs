using DLeon_Asociados_Web.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations;

[DbContext(typeof(Contexto))]
[Migration("20261106182000_AgregarProveedoresIniciales")]
public partial class AgregarProveedoresIniciales : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            INSERT INTO [Proveedores]
                ([Existe], [Nombre], [Documento], [Telefono], [Correo], [Ciudad], [Estado], [Direccion])
            SELECT seed.[Existe], seed.[Nombre], seed.[Documento], seed.[Telefono],
                   seed.[Correo], seed.[Ciudad], seed.[Estado], seed.[Direccion]
            FROM (VALUES
                (CAST(1 AS bit), N'Saul', N'000-0000001-1', N'809-000-0001', N'saul@ejemplo.com', N'Santo Domingo', N'Activo', N''),
                (CAST(1 AS bit), N'Euwin', N'000-0000002-2', N'809-000-0002', N'euwin@ejemplo.com', N'Puerto Plata', N'Activo', N''),
                (CAST(1 AS bit), N'Estharlyn', N'000-0000003-3', N'809-000-0003', N'estharlyn@ejemplo.com', N'Tenares', N'Activo', N''),
                (CAST(1 AS bit), N'Andy', N'000-0000004-4', N'809-000-0004', N'andy@ejemplo.com', N'Santo Domingo', N'Activo', N''),
                (CAST(1 AS bit), N'Elian', N'000-0000005-5', N'809-000-0005', N'elian@ejemplo.com', N'Santo Domingo', N'Activo', N'')
            ) AS seed ([Existe], [Nombre], [Documento], [Telefono], [Correo], [Ciudad], [Estado], [Direccion])
            WHERE NOT EXISTS (
                SELECT 1
                FROM [Proveedores] AS existing
                WHERE existing.[Documento] = seed.[Documento]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM [Proveedores]
            WHERE ([Documento] = N'000-0000001-1' AND [Nombre] = N'Saul')
               OR ([Documento] = N'000-0000002-2' AND [Nombre] = N'Euwin')
               OR ([Documento] = N'000-0000003-3' AND [Nombre] = N'Estharlyn')
               OR ([Documento] = N'000-0000004-4' AND [Nombre] = N'Andy')
               OR ([Documento] = N'000-0000005-5' AND [Nombre] = N'Elian');
            """);
    }
}
