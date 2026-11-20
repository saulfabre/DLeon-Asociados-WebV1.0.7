using System.ComponentModel.DataAnnotations;
using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public class RepuestosServices(IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<Repuestos>> ObtenerTodos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Repuestos
            .AsNoTracking()
            .Include(repuesto => repuesto.ProveedorDetalle)
            .Include(repuesto => repuesto.DetallesProveedores).ThenInclude(detalle => detalle.Proveedor)
            .Where(repuesto => repuesto.Existe)
            .OrderBy(repuesto => repuesto.Nombre)
            .ToListAsync();
    }

    public async Task<Repuestos> Crear(Repuestos repuesto, IReadOnlyCollection<int>? proveedoresIds = null)
    {
        ArgumentNullException.ThrowIfNull(repuesto);
        Validar(repuesto);

        repuesto.Existe = true;
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        await AsignarProveedor(contexto, repuesto);
        repuesto.ProveedorDetalle = null;
        contexto.Repuestos.Add(repuesto);
        await contexto.SaveChangesAsync();
        await SincronizarProveedores(contexto, repuesto, proveedoresIds ?? (repuesto.ProveedorId.HasValue ? [repuesto.ProveedorId.Value] : []));
        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();
        return repuesto;
    }

    public async Task<bool> Actualizar(Repuestos repuesto, IReadOnlyCollection<int>? proveedoresIds = null)
    {
        ArgumentNullException.ThrowIfNull(repuesto);
        Validar(repuesto);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        var existente = await contexto.Repuestos
            .FirstOrDefaultAsync(item => item.Id == repuesto.Id && item.Existe);

        if (existente is null)
        {
            return false;
        }

        await AsignarProveedor(contexto, repuesto);
        existente.Nombre = repuesto.Nombre.Trim();
        existente.Marca = repuesto.Marca.Trim();
        existente.Tipo = repuesto.Tipo.Trim();
        existente.Tamano = NormalizarOpcional(repuesto.Tamano);
        existente.Disponibles = repuesto.Disponibles;
        existente.PrecioUnitario = repuesto.PrecioUnitario;
        existente.Proveedor = NormalizarOpcional(repuesto.Proveedor);
        existente.ProveedorId = repuesto.ProveedorId;

        await contexto.Entry(existente).Collection(item => item.DetallesProveedores).LoadAsync();
        await SincronizarProveedores(contexto, existente, proveedoresIds ?? (repuesto.ProveedorId.HasValue ? [repuesto.ProveedorId.Value] : []));

        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();
        return true;
    }

    public async Task<List<Proveedores>> ObtenerProveedoresDisponibles()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Proveedores.AsNoTracking().Where(item => item.Existe && item.Estado == "Activo").OrderBy(item => item.Nombre).ToListAsync();
    }

    private static async Task SincronizarProveedores(Contexto contexto, Repuestos repuesto, IReadOnlyCollection<int> ids)
    {
        var deseados = ids.Distinct().ToHashSet();
        var validos = await contexto.Proveedores.Where(item => item.Existe && item.Estado == "Activo" && deseados.Contains(item.Id)).Select(item => item.Id).ToListAsync();
        if (validos.Count != deseados.Count) throw new InvalidOperationException("Selecciona proveedores activos registrados.");
        foreach (var actual in repuesto.DetallesProveedores.Where(item => !deseados.Contains(item.ProveedorId)).ToList()) contexto.ProveedoresRepuestos.Remove(actual);
        foreach (var id in deseados.Where(id => repuesto.DetallesProveedores.All(item => item.ProveedorId != id)))
            repuesto.DetallesProveedores.Add(new ProveedorRepuesto { Repuesto = repuesto, ProveedorId = id });
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var repuesto = await contexto.Repuestos
            .FirstOrDefaultAsync(item => item.Id == id && item.Existe);

        if (repuesto is null)
        {
            return false;
        }

        repuesto.Existe = false;
        await contexto.SaveChangesAsync();
        return true;
    }

    private static void Validar(Repuestos repuesto)
    {
        repuesto.Nombre = repuesto.Nombre.Trim();
        repuesto.Marca = repuesto.Marca.Trim();
        repuesto.Tipo = repuesto.Tipo.Trim();
        repuesto.Tamano = NormalizarOpcional(repuesto.Tamano);
        repuesto.Proveedor = NormalizarOpcional(repuesto.Proveedor);

        var errores = new List<ValidationResult>();
        var contextoValidacion = new ValidationContext(repuesto);
        if (!Validator.TryValidateObject(repuesto, contextoValidacion, errores, validateAllProperties: true))
        {
            throw new InvalidOperationException(string.Join(" ", errores.Select(error => error.ErrorMessage)));
        }
    }

    private static string? NormalizarOpcional(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static async Task AsignarProveedor(Contexto contexto, Repuestos repuesto)
    {
        if (repuesto.ProveedorId is null)
        {
            repuesto.Proveedor = NormalizarOpcional(repuesto.Proveedor);
            return;
        }

        var proveedor = await contexto.Proveedores
            .FirstOrDefaultAsync(item => item.Id == repuesto.ProveedorId && item.Existe);
        if (proveedor is null)
        {
            throw new InvalidOperationException("Selecciona un proveedor registrado.");
        }

        if (proveedor.Estado != "Activo")
        {
            var proveedorActualId = repuesto.Id == 0
                ? null
                : await contexto.Repuestos
                    .Where(item => item.Id == repuesto.Id && item.Existe)
                    .Select(item => item.ProveedorId)
                    .FirstOrDefaultAsync();
            if (proveedorActualId != proveedor.Id)
            {
                throw new InvalidOperationException("Solo puedes asignar repuestos a proveedores activos.");
            }
        }

        repuesto.Proveedor = proveedor.Nombre;
    }
}
