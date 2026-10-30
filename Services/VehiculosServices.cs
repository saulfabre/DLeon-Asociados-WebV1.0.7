using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public class VehiculosServices(IDbContextFactory<Contexto> contextFactory)
{
    public async Task<Vehiculos?> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Vehiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && v.Existe);
    }

    public async Task<List<Vehiculos>> GetList(
        string? marca = null,
        string? modelo = null,
        string? tipo = null,
        string? estado = null,
        int? anioDesde = null,
        int? anioHasta = null,
        decimal? precioMinimo = null,
        decimal? precioMaximo = null)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var query = contexto.Vehiculos
            .AsNoTracking()
            .Where(v => v.Existe)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(marca))
        {
            query = query.Where(v => v.Marca == marca);
        }

        if (!string.IsNullOrWhiteSpace(modelo))
        {
            query = query.Where(v => v.Modelo == modelo);
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            query = query.Where(v => v.Tipo == tipo);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(v => v.Estado == estado);
        }

        if (anioDesde.HasValue)
        {
            query = query.Where(v => v.Anio >= anioDesde.Value);
        }

        if (anioHasta.HasValue)
        {
            query = query.Where(v => v.Anio <= anioHasta.Value);
        }

        if (precioMinimo.HasValue)
        {
            query = query.Where(v => v.Precio >= precioMinimo.Value);
        }

        if (precioMaximo.HasValue)
        {
            query = query.Where(v => v.Precio <= precioMaximo.Value);
        }

        return await query
            .OrderByDescending(v => v.FechaPublicacion ?? v.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Vehiculos> Insertar(Vehiculos vehiculo)
    {
        ArgumentNullException.ThrowIfNull(vehiculo);

        if (string.IsNullOrWhiteSpace(vehiculo.Marca) ||
            string.IsNullOrWhiteSpace(vehiculo.Modelo) ||
            string.IsNullOrWhiteSpace(vehiculo.Matricula) ||
            vehiculo.Anio <= 0)
        {
            throw new InvalidOperationException("La marca, el modelo, la matrícula y el año del vehículo son obligatorios.");
        }

        if (await Existe(vehiculo.Marca, vehiculo.Modelo, vehiculo.Anio, vehiculo.Edicion, vehiculo.Tipo))
        {
            throw new InvalidOperationException("Ya existe un vehículo con la misma marca, modelo, año y edición.");
        }

        if (string.IsNullOrWhiteSpace(vehiculo.Nombre))
        {
            vehiculo.Nombre = string.Join(' ', new[]
            {
                vehiculo.Marca,
                vehiculo.Modelo,
                vehiculo.Edicion
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        vehiculo.Matricula = vehiculo.Matricula.Trim();

        if (vehiculo.FechaPublicacion == null)
        {
            vehiculo.FechaPublicacion = DateTime.UtcNow;
        }

        if (vehiculo.FechaCreacion == default)
        {
            vehiculo.FechaCreacion = DateTime.UtcNow;
        }

        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Vehiculos.Add(vehiculo);
        await contexto.SaveChangesAsync();

        return vehiculo;
    }

    public async Task<bool> Modificar(Vehiculos vehiculo)
    {
        ArgumentNullException.ThrowIfNull(vehiculo);

        if (string.IsNullOrWhiteSpace(vehiculo.Marca) ||
            string.IsNullOrWhiteSpace(vehiculo.Modelo) ||
            string.IsNullOrWhiteSpace(vehiculo.Matricula) ||
            string.IsNullOrWhiteSpace(vehiculo.Tipo) ||
            vehiculo.Anio <= 0)
        {
            throw new InvalidOperationException("La marca, el modelo, la matrícula, el tipo y el año del vehículo son obligatorios.");
        }

        if (await Existe(
            vehiculo.Marca,
            vehiculo.Modelo,
            vehiculo.Anio,
            vehiculo.Edicion,
            vehiculo.Tipo,
            vehiculo.Id))
        {
            throw new InvalidOperationException("Ya existe otro vehículo con la misma marca, modelo, año y edición.");
        }

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.Vehiculos
            .FirstOrDefaultAsync(v => v.Id == vehiculo.Id && v.Existe);

        if (existente is null)
        {
            return false;
        }

        existente.Marca = vehiculo.Marca;
        existente.Modelo = vehiculo.Modelo;
        existente.Matricula = vehiculo.Matricula.Trim();
        existente.Edicion = vehiculo.Edicion;
        existente.Tipo = vehiculo.Tipo;
        existente.Anio = vehiculo.Anio;
        existente.Precio = vehiculo.Precio;
        existente.Estado = vehiculo.Estado;
        existente.Color = vehiculo.Color;
        existente.Combustible = vehiculo.Combustible;
        existente.Transmision = vehiculo.Transmision;
        existente.Kilometraje = vehiculo.Kilometraje;
        existente.Descripcion = vehiculo.Descripcion;
        existente.Imagen = vehiculo.Imagen;
        existente.Nombre = string.IsNullOrWhiteSpace(vehiculo.Nombre)
            ? string.Join(' ', new[]
            {
                vehiculo.Marca,
                vehiculo.Modelo,
                vehiculo.Edicion
            }.Where(value => !string.IsNullOrWhiteSpace(value)))
            : vehiculo.Nombre;

        if (vehiculo.FechaPublicacion.HasValue)
        {
            existente.FechaPublicacion = vehiculo.FechaPublicacion;
        }

        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var vehiculo = await contexto.Vehiculos
            .FirstOrDefaultAsync(v => v.Id == id && v.Existe);

        if (vehiculo is null)
        {
            return false;
        }

        vehiculo.Existe = false;
        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Existe(
        string marca,
        string modelo,
        int anio,
        string? edicion = null,
        string? tipo = null,
        int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(marca) || string.IsNullOrWhiteSpace(modelo))
        {
            return false;
        }

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var query = contexto.Vehiculos.AsNoTracking().Where(v =>
            v.Existe &&
            v.Marca == marca &&
            v.Modelo == modelo &&
            v.Anio == anio &&
            v.Edicion == edicion &&
            v.Tipo == tipo);

        if (excluirId.HasValue)
        {
            query = query.Where(v => v.Id != excluirId.Value);
        }

        return await query.AnyAsync();
    }
}
