using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public class ClientesServices(IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<Clientes>> ObtenerTodos(string? busqueda = null)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var query = contexto.Clientes.AsNoTracking().Where(cliente => cliente.Existe).AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            query = query.Where(cliente =>
                cliente.NombreCompleto.Contains(termino) ||
                (cliente.Documento != null && cliente.Documento.Contains(termino)) ||
                (cliente.Telefono != null && cliente.Telefono.Contains(termino)) ||
                (cliente.Correo != null && cliente.Correo.Contains(termino)));
        }

        return SepararNombres(await query.OrderBy(cliente => cliente.NombreCompleto).ToListAsync());
    }

    public async Task<Clientes?> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var cliente = await contexto.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id && item.Existe);
        return cliente is null ? null : SepararNombre(cliente);
    }

    public async Task<Clientes> Crear(Clientes cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);
        NormalizarYValidar(cliente);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        if (await contexto.Clientes.AnyAsync(existente => existente.Existe && existente.Documento == cliente.Documento))
        {
            throw new InvalidOperationException("Ya existe un cliente con ese documento.");
        }

        cliente.NombreCompleto = string.Join(' ', new[] { cliente.Nombre, cliente.Apellido });
        contexto.Clientes.Add(cliente);
        await contexto.SaveChangesAsync();
        return cliente;
    }

    public async Task<bool> Actualizar(Clientes cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);
        NormalizarYValidar(cliente);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        var existente = await contexto.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id && c.Existe);
        if (existente is null)
        {
            return false;
        }

        if (await contexto.Clientes.AnyAsync(c => c.Existe && c.Id != cliente.Id && c.Documento == cliente.Documento))
        {
            throw new InvalidOperationException("Ya existe otro cliente con ese documento.");
        }

        existente.Nombre = cliente.Nombre;
        existente.NombreCompleto = string.Join(' ', new[] { cliente.Nombre, cliente.Apellido });
        existente.Documento = cliente.Documento;
        existente.Telefono = cliente.Telefono;
        existente.Correo = cliente.Correo;
        existente.Direccion = cliente.Direccion;
        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var cliente = await contexto.Clientes.FirstOrDefaultAsync(c => c.Id == id && c.Existe);
        if (cliente is null)
        {
            return false;
        }

        cliente.Existe = false;
        await contexto.SaveChangesAsync();
        return true;
    }

    private static void NormalizarYValidar(Clientes cliente)
    {
        cliente.Nombre = cliente.Nombre.Trim();
        cliente.Apellido = cliente.Apellido.Trim();
        cliente.Documento = cliente.Documento?.Trim();
        cliente.Telefono = cliente.Telefono?.Trim();
        cliente.Correo = cliente.Correo?.Trim();
        cliente.Direccion = cliente.Direccion?.Trim();

        if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
            string.IsNullOrWhiteSpace(cliente.Apellido) ||
            string.IsNullOrWhiteSpace(cliente.Documento) ||
            string.IsNullOrWhiteSpace(cliente.Telefono))
        {
            throw new InvalidOperationException("Nombre, apellido, documento y teléfono son obligatorios.");
        }
    }

    private static List<Clientes> SepararNombres(List<Clientes> clientes)
    {
        foreach (var cliente in clientes)
        {
            SepararNombre(cliente);
        }

        return clientes;
    }

    private static Clientes SepararNombre(Clientes cliente)
    {
        var partes = cliente.NombreCompleto.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        cliente.Nombre = partes.FirstOrDefault() ?? string.Empty;
        cliente.Apellido = partes.Length > 1 ? partes[1] : string.Empty;
        return cliente;
    }
}