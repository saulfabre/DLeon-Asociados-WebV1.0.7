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
            .Where(repuesto => repuesto.Existe)
            .OrderBy(repuesto => repuesto.Nombre)
            .ToListAsync();
    }

    public async Task<Repuestos> Crear(Repuestos repuesto)
    {
        ArgumentNullException.ThrowIfNull(repuesto);
        Validar(repuesto);

        repuesto.Existe = true;
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Repuestos.Add(repuesto);
        await contexto.SaveChangesAsync();
        return repuesto;
    }

    public async Task<bool> Actualizar(Repuestos repuesto)
    {
        ArgumentNullException.ThrowIfNull(repuesto);
        Validar(repuesto);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        var existente = await contexto.Repuestos
            .FirstOrDefaultAsync(item => item.Id == repuesto.Id && item.Existe);

        if (existente is null)
        {
            return false;
        }

        existente.Nombre = repuesto.Nombre.Trim();
        existente.Marca = repuesto.Marca.Trim();
        existente.Tipo = repuesto.Tipo.Trim();
        existente.Tamano = NormalizarOpcional(repuesto.Tamano);
        existente.Disponibles = repuesto.Disponibles;
        existente.PrecioUnitario = repuesto.PrecioUnitario;
        existente.Proveedor = NormalizarOpcional(repuesto.Proveedor);

        await contexto.SaveChangesAsync();
        return true;
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
}
