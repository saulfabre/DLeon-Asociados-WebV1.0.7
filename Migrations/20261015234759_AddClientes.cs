using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Seguros",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Clientes] (
                        [IdCliente] int IDENTITY(1,1) NOT NULL,
                        [NombreCompleto] varchar(160) NOT NULL,
                        [Documento] varchar(40) NULL,
                        [Telefono] varchar(30) NULL,
                        [Correo] varchar(150) NULL,
                        [Direccion] varchar(250) NULL,
                        CONSTRAINT [PK_Clientes] PRIMARY KEY ([IdCliente])
                    );
                END
                ELSE
                BEGIN
                    IF COL_LENGTH(N'dbo.Clientes', N'Documento') IS NULL
                        ALTER TABLE [dbo].[Clientes] ADD [Documento] varchar(40) NULL;

                    ALTER TABLE [dbo].[Clientes] ALTER COLUMN [NombreCompleto] varchar(160) NOT NULL;
                    ALTER TABLE [dbo].[Clientes] ALTER COLUMN [Telefono] varchar(30) NULL;
                    ALTER TABLE [dbo].[Clientes] ALTER COLUMN [Correo] varchar(150) NULL;
                    ALTER TABLE [dbo].[Clientes] ALTER COLUMN [Direccion] varchar(250) NULL;
                END
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Seguros_ClienteId",
                table: "Seguros",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes",
                column: "Documento",
                unique: true,
                filter: "[Documento] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Seguros_Clientes_ClienteId",
                table: "Seguros",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seguros_Clientes_ClienteId",
                table: "Seguros");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Seguros_ClienteId",
                table: "Seguros");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Seguros");
        }
    }
}
