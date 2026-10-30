using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public class GarantiasServices(IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<Garantias>> ObtenerTodos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Garantias
            .AsNoTracking()
            .Include(garantia => garantia.Vehiculo)
            .Where(garantia => garantia.Existe)
            .OrderBy(garantia => garantia.FechaExpiracion)
            .ToListAsync();
    }

    public async Task<Garantias> Crear(Garantias garantia)
    {
        ArgumentNullException.ThrowIfNull(garantia);
        Validar(garantia);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        if (!await contexto.Vehiculos.AnyAsync(vehiculo =>
                vehiculo.Id == garantia.VehiculoId && vehiculo.Existe))
        {
            throw new InvalidOperationException("El vehículo seleccionado ya no existe.");
        }

        garantia.Vehiculo = null;
        contexto.Garantias.Add(garantia);
        await contexto.SaveChangesAsync();
        return garantia;
    }

    public async Task<bool> Actualizar(Garantias garantia)
    {
        ArgumentNullException.ThrowIfNull(garantia);
        Validar(garantia);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        var existente = await contexto.Garantias
            .FirstOrDefaultAsync(item => item.Id == garantia.Id && item.Existe);
        if (existente is null)
        {
            return false;
        }

        if (existente.VehiculoId != garantia.VehiculoId &&
            !await contexto.Vehiculos.AnyAsync(vehiculo =>
                vehiculo.Id == garantia.VehiculoId && vehiculo.Existe))
        {
            throw new InvalidOperationException("El vehículo seleccionado ya no existe.");
        }

        existente.VehiculoId = garantia.VehiculoId;
        existente.Tipo = garantia.Tipo.Trim();
        existente.FechaInicio = garantia.FechaInicio.Date;
        existente.FechaExpiracion = garantia.FechaExpiracion.Date;
        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var garantia = await contexto.Garantias
            .FirstOrDefaultAsync(item => item.Id == id && item.Existe);
        if (garantia is null)
        {
            return false;
        }

        garantia.Existe = false;
        await contexto.SaveChangesAsync();
        return true;
    }

    private static void Validar(Garantias garantia)
    {
        if (garantia.VehiculoId <= 0 || string.IsNullOrWhiteSpace(garantia.Tipo))
        {
            throw new InvalidOperationException("El vehículo y el tipo de garantía son obligatorios.");
        }

        if (garantia.FechaInicio == default ||
            garantia.FechaExpiracion == default ||
            garantia.FechaExpiracion.Date < garantia.FechaInicio.Date)
        {
            throw new InvalidOperationException("La fecha de expiración debe ser igual o posterior a la fecha de inicio.");
        }

        garantia.Tipo = garantia.Tipo.Trim();
    }
}
