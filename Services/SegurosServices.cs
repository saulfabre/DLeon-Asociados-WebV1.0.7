using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public class SegurosServices(IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<Seguros>> ObtenerTodos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Seguros
            .AsNoTracking()
            .Include(seguro => seguro.Vehiculo)
            .Where(seguro => seguro.Existe)
            .OrderBy(seguro => seguro.FechaExpiracion)
            .ToListAsync();
    }

    public async Task<Seguros?> ObtenerPorId(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Seguros
            .AsNoTracking()
            .Include(seguro => seguro.Vehiculo)
            .FirstOrDefaultAsync(seguro => seguro.Id == id && seguro.Existe);
    }

    public async Task<List<Seguros>> Buscar(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            return await ObtenerTodos();
        }

        var busqueda = termino.Trim();
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Seguros
            .AsNoTracking()
            .Include(seguro => seguro.Vehiculo)
            .Where(seguro => seguro.Existe &&
                (seguro.NumeroSeguro.Contains(busqueda) ||
                seguro.ProveedorNombre.Contains(busqueda) ||
                seguro.TipoProveedor.Contains(busqueda) ||
                seguro.Tipo.Contains(busqueda) ||
                seguro.Vehiculo!.Marca.Contains(busqueda) ||
                seguro.Vehiculo.Modelo.Contains(busqueda) ||
                seguro.Vehiculo.Nombre.Contains(busqueda) ||
                seguro.Vehiculo.Matricula.Contains(busqueda)))
            .OrderBy(seguro => seguro.FechaExpiracion)
            .ToListAsync();
    }

    public async Task<Seguros> Crear(Seguros seguro)
    {
        ArgumentNullException.ThrowIfNull(seguro);
        Validar(seguro);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        if (!await contexto.Vehiculos.AnyAsync(vehiculo => vehiculo.Id == seguro.VehiculoId && vehiculo.Existe))
        {
            throw new InvalidOperationException("El vehículo seleccionado ya no existe o fue eliminado.");
        }

        if (seguro.ClienteId.HasValue &&
            !await contexto.Clientes.AnyAsync(cliente => cliente.Id == seguro.ClienteId.Value && cliente.Existe))
        {
            throw new InvalidOperationException("El cliente seleccionado ya no existe.");
        }

        seguro.Vehiculo = null;
        seguro.Cliente = null;
        contexto.Seguros.Add(seguro);
        await contexto.SaveChangesAsync();
        return seguro;
    }

    public async Task<bool> Actualizar(Seguros seguro)
    {
        ArgumentNullException.ThrowIfNull(seguro);
        Validar(seguro);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        var existente = await contexto.Seguros.FirstOrDefaultAsync(item => item.Id == seguro.Id && item.Existe);
        if (existente is null)
        {
            return false;
        }

        if (existente.VehiculoId != seguro.VehiculoId &&
            !await contexto.Vehiculos.AnyAsync(vehiculo => vehiculo.Id == seguro.VehiculoId && vehiculo.Existe))
        {
            throw new InvalidOperationException("El vehículo seleccionado ya no existe o fue eliminado.");
        }

        if (seguro.ClienteId.HasValue &&
            seguro.ClienteId != existente.ClienteId &&
            !await contexto.Clientes.AnyAsync(cliente => cliente.Id == seguro.ClienteId.Value && cliente.Existe))
        {
            throw new InvalidOperationException("El cliente seleccionado ya no existe.");
        }

        existente.VehiculoId = seguro.VehiculoId;
        existente.ClienteId = seguro.ClienteId;
        existente.Tipo = seguro.Tipo.Trim();
        existente.NumeroSeguro = seguro.NumeroSeguro.Trim();
        existente.TipoProveedor = seguro.TipoProveedor.Trim();
        existente.ProveedorNombre = seguro.ProveedorNombre.Trim();
        existente.FechaInicio = seguro.FechaInicio.Date;
        existente.FechaExpiracion = seguro.FechaExpiracion.Date;
        existente.ClienteNombre = seguro.ClienteNombre.Trim();
        existente.ClienteApellido = seguro.ClienteApellido.Trim();
        existente.ClienteDocumento = seguro.ClienteDocumento.Trim();
        existente.ClienteTelefono = seguro.ClienteTelefono.Trim();
        existente.ClienteCorreo = seguro.ClienteCorreo.Trim();
        existente.ClienteDireccion = seguro.ClienteDireccion.Trim();
        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var seguro = await contexto.Seguros.FirstOrDefaultAsync(item => item.Id == id && item.Existe);
        if (seguro is null)
        {
            return false;
        }

        seguro.Existe = false;
        await contexto.SaveChangesAsync();
        return true;
    }

    private static void Validar(Seguros seguro)
    {
        var errores = new List<ValidationResult>();
        var contextoValidacion = new ValidationContext(seguro);
        if (!Validator.TryValidateObject(seguro, contextoValidacion, errores, validateAllProperties: true))
        {
            throw new InvalidOperationException(string.Join(" ", errores.Select(error => error.ErrorMessage)));
        }

        if (seguro.VehiculoId <= 0 || string.IsNullOrWhiteSpace(seguro.Tipo))
        {
            throw new InvalidOperationException("El vehículo y el tipo de seguro son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(seguro.NumeroSeguro) ||
            string.IsNullOrWhiteSpace(seguro.TipoProveedor) ||
            string.IsNullOrWhiteSpace(seguro.ProveedorNombre))
        {
            throw new InvalidOperationException("El número, el proveedor y el origen del proveedor son obligatorios.");
        }

        if (seguro.TipoProveedor is not ("Propio" or "Externo"))
        {
            throw new InvalidOperationException("El origen del proveedor debe ser Propio o Externo.");
        }

        if (seguro.Tipo is not ("Estandar" or "Semi-Full" or "Full"))
        {
            throw new InvalidOperationException("El tipo de seguro debe ser Estandar, Semi-Full o Full.");
        }

        if (seguro.FechaInicio == default || seguro.FechaExpiracion == default ||
            seguro.FechaExpiracion.Date < seguro.FechaInicio.Date)
        {
            throw new InvalidOperationException("La fecha de expiración debe ser igual o posterior a la fecha de inicio.");
        }

        seguro.Tipo = seguro.Tipo.Trim();
        seguro.NumeroSeguro = seguro.NumeroSeguro.Trim();
        seguro.TipoProveedor = seguro.TipoProveedor.Trim();
        seguro.ProveedorNombre = seguro.ProveedorNombre.Trim();
        seguro.ClienteNombre = seguro.ClienteNombre.Trim();
        seguro.ClienteApellido = seguro.ClienteApellido.Trim();
        seguro.ClienteDocumento = seguro.ClienteDocumento.Trim();
        seguro.ClienteTelefono = seguro.ClienteTelefono.Trim();
        seguro.ClienteCorreo = seguro.ClienteCorreo.Trim();
        seguro.ClienteDireccion = seguro.ClienteDireccion.Trim();
    }
}