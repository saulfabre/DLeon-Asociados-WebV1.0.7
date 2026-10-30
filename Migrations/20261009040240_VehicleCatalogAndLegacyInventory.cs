using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DLeon_Asociados_Web.Migrations
{
    /// <inheritdoc />
    public partial class VehicleCatalogAndLegacyInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DECLARE @fecha datetime2 = SYSUTCDATETIME();

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'Hilux' AND [Anio] = 2027)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'Hilux', N'Pick-up', 2027, 84000, N'Disponible', N'Gasolina', N'La Toyota Hilux Travo-e 2027 es una camioneta pick-up 100% eléctrica (BEV) que maximiza la resistencia tradicional de la línea Hilux con tecnología de movilidad sostenible.', N'/images/vehiculos/toyota-hilux.jpg', N'2027 Toyota Hilux', @fecha, @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'4Runner' AND [Anio] = 2025)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'4Runner', N'SUV', 2025, 70000, N'Disponible', N'Gasolina', N'Toyota 4Runner modelo 2025. Vehículo disponible para venta.', N'/images/vehiculos/toyota-4runner.jpg', N'2025 Toyota 4Runner', DATEADD(second, -1, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'Tacoma' AND [Anio] = 2022)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'Tacoma', N'Pick-up', 2022, 38900, N'Disponible', N'Gasolina', N'Toyota Tacoma modelo 2022. Vehículo disponible para venta.', N'/images/vehiculos/toyota-tacoma.webp', N'2022 Toyota Tacoma', DATEADD(second, -2, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Toyota' AND [Modelo] = N'Corolla' AND [Anio] = 2023)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Toyota', N'Corolla', N'Sedán', 2023, 25500, N'Disponible', N'Gasolina', N'Toyota Corolla modelo 2023. Vehículo disponible para venta.', N'/images/vehiculos/toyota-corolla.webp', N'2023 Toyota Corolla', DATEADD(second, -3, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'BMW' AND [Modelo] = N'X7' AND [Anio] = 2023)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'BMW', N'X7', N'SUV', 2023, 115200, N'Disponible', N'Gasolina', N'BMW X7 modelo 2023. Vehículo disponible para venta.', N'/images/vehiculos/bmw-x7.jpg', N'2023 BMW X7', DATEADD(second, -4, @fecha), @fecha);

                IF NOT EXISTS (SELECT 1 FROM [Vehiculos] WHERE [Marca] = N'Kia' AND [Modelo] = N'K5' AND [Anio] = 2023)
                    INSERT INTO [Vehiculos] ([Marca], [Modelo], [Tipo], [Anio], [Precio], [Estado], [Combustible], [Descripcion], [Imagen], [Nombre], [FechaPublicacion], [FechaCreacion])
                    VALUES (N'Kia', N'K5', N'Sedán', 2023, 15000, N'Disponible', N'Gasolina', N'Kia K5 modelo 2023. Vehículo disponible para venta.', N'/images/vehiculos/kia-k5.jpeg', N'2023 Kia K5', DATEADD(second, -5, @fecha), @fecha);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Marca_Modelo",
                table: "Vehiculos",
                columns: new[] { "Marca", "Modelo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_Marca_Modelo",
                table: "Vehiculos");

            migrationBuilder.Sql("""
                DELETE FROM [Vehiculos]
                WHERE ([Marca] = N'Toyota' AND [Modelo] = N'Hilux' AND [Anio] = 2027 AND [Descripcion] = N'La Toyota Hilux Travo-e 2027 es una camioneta pick-up 100% eléctrica (BEV) que maximiza la resistencia tradicional de la línea Hilux con tecnología de movilidad sostenible.')
                   OR ([Marca] = N'Toyota' AND [Modelo] = N'4Runner' AND [Anio] = 2025 AND [Descripcion] = N'Toyota 4Runner modelo 2025. Vehículo disponible para venta.')
                   OR ([Marca] = N'Toyota' AND [Modelo] = N'Tacoma' AND [Anio] = 2022 AND [Descripcion] = N'Toyota Tacoma modelo 2022. Vehículo disponible para venta.')
                   OR ([Marca] = N'Toyota' AND [Modelo] = N'Corolla' AND [Anio] = 2023 AND [Descripcion] = N'Toyota Corolla modelo 2023. Vehículo disponible para venta.')
                   OR ([Marca] = N'BMW' AND [Modelo] = N'X7' AND [Anio] = 2023 AND [Descripcion] = N'BMW X7 modelo 2023. Vehículo disponible para venta.')
                   OR ([Marca] = N'Kia' AND [Modelo] = N'K5' AND [Anio] = 2023 AND [Descripcion] = N'Kia K5 modelo 2023. Vehículo disponible para venta.');
                """);
        }
    }
}
