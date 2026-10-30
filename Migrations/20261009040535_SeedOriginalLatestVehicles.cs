using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedOriginalLatestVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DECLARE @fecha datetime2 = DATEADD(second, 10, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'BMW' AND [Modelo] = N'X7' AND [Anio] = 2022)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Edicion], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Transmision], [Kilometraje], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'BMW', N'X7', N'xDrive40i', N'Jeepeta', 2022, 5200000, N'Disponible', N'Gasolina', N'Automático', 28500, N'Registro inicial del catálogo público: BMW X7 xDrive40i.', N'/images/imagesSaul/public/bmw-x7.jpg.jpeg', N'2022 BMW X7 xDrive40i', @fecha, @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Kia' AND [Modelo] = N'K5' AND [Anio] = 2022)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Edicion], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Transmision], [Kilometraje], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Kia', N'K5', N'GT-Line', N'Sedán', 2022, 1450000, N'Disponible', N'Gasolina', N'Automático', 24000, N'Registro inicial del catálogo público: Kia K5 GT-Line.', N'/images/imagesSaul/public/kia-k5.jpeg', N'2022 Kia K5 GT-Line', DATEADD(second, -1, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'4Runner' AND [Anio] = 2022)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Edicion], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Transmision], [Kilometraje], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'4Runner', N'SR5', N'Jeepeta', 2022, 3850000, N'Disponible', N'Gasolina', N'Automático', 32000, N'Registro inicial del catálogo público: Toyota 4Runner SR5.', N'/images/imagesSaul/public/toyota-4runner.jpeg', N'2022 Toyota 4Runner SR5', DATEADD(second, -2, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'Corolla' AND [Anio] = 2019)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Edicion], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Transmision], [Kilometraje], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'Corolla', N'LE', N'Sedán', 2019, 890000, N'Disponible', N'Gasolina', N'Automático', 56800, N'Registro inicial del catálogo público: Toyota Corolla LE.', N'/images/imagesSaul/public/toyota-corolla.webp', N'2019 Toyota Corolla LE', DATEADD(second, -3, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'Hilux' AND [Anio] = 2022)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Edicion], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Transmision], [Kilometraje], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'Hilux', N'SR', N'Camioneta', 2022, 1490000, N'Disponible', N'Diésel', N'Manual', 18600, N'Registro inicial del catálogo público: Toyota Hilux SR.', N'/images/imagesSaul/public/toyota-hilux.jpg', N'2022 Toyota Hilux SR', DATEADD(second, -4, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'Tacoma' AND [Anio] = 2021)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Edicion], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Transmision], [Kilometraje], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'Tacoma', N'SR5', N'Camioneta', 2021, 1500000, N'Disponible', N'Gasolina', N'Automático', 35000, N'Registro inicial del catálogo público: Toyota Tacoma SR5.', N'/images/imagesSaul/public/toyota-tacoma.webp', N'2021 Toyota Tacoma SR5', DATEADD(second, -5, @fecha), @fecha);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Estado",
                table: "Vehiculos",
                column: "Estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_Estado",
                table: "Vehiculos");

            migrationBuilder.Sql("""
                DELETE FROM [Vehiculos]
                WHERE [Descripcion] LIKE N'Registro inicial del catálogo público:%';
                """);
        }
    }
}
