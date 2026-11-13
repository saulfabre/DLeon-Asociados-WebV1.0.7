using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using DLeon_Asociados_Web.Context;
using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Services;

public sealed record ProveedoresCatalogos(
    IReadOnlyCollection<string> Marcas,
    IReadOnlyCollection<string> Modelos,
    IReadOnlyCollection<string> Categorias,
    IReadOnlyCollection<RepuestoOfertaProveedor> RepuestosOfrecidos);

public sealed class ProveedoresStore(IDbContextFactory<Contexto> contextFactory)
{
    private static readonly string[] TerminosPagoValidos = ["Contado", "Crédito", "Transferencia"];

    public async Task<List<Proveedores>> ObtenerTodos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Proveedores
            .AsNoTracking()
            .Where(proveedor => proveedor.Existe)
            .OrderBy(proveedor => proveedor.Nombre)
            .ToListAsync();
    }

    public async Task<Proveedores?> ObtenerPorId(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Proveedores
            .AsNoTracking()
            .AsSplitQuery()
            .Include(proveedor => proveedor.Marcas)
                .ThenInclude(relacion => relacion.Marca)
            .Include(proveedor => proveedor.ModelosVehiculos)
                .ThenInclude(relacion => relacion.ModeloVehiculo)
            .Include(proveedor => proveedor.CategoriasPiezas)
                .ThenInclude(relacion => relacion.CategoriaPieza)
            .Include(proveedor => proveedor.CatalogoRepuestos)
            .Include(proveedor => proveedor.Repuestos.Where(repuesto => repuesto.Existe))
            .FirstOrDefaultAsync(proveedor => proveedor.Id == id && proveedor.Existe);
    }

    public async Task<Proveedores> Crear(Proveedores proveedor, ProveedoresCatalogos catalogos)
    {
        ArgumentNullException.ThrowIfNull(proveedor);
        ArgumentNullException.ThrowIfNull(catalogos);
        catalogos = NormalizarCatalogos(catalogos);
        Validar(proveedor);
        ValidarCatalogos(catalogos);

        proveedor.Existe = true;
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        contexto.Proveedores.Add(proveedor);
        await contexto.SaveChangesAsync();
        await SincronizarCatalogos(contexto, proveedor, catalogos);
        SincronizarRepuestosOfrecidos(contexto, proveedor, proveedor.CatalogoRepuestos, catalogos.RepuestosOfrecidos);
        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();
        return proveedor;
    }

    public async Task<bool> Actualizar(Proveedores proveedor, ProveedoresCatalogos catalogos)
    {
        ArgumentNullException.ThrowIfNull(proveedor);
        ArgumentNullException.ThrowIfNull(catalogos);
        catalogos = NormalizarCatalogos(catalogos);
        Validar(proveedor);
        ValidarCatalogos(catalogos);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        var existente = await contexto.Proveedores
            .Include(item => item.Marcas)
            .Include(item => item.ModelosVehiculos)
            .Include(item => item.CategoriasPiezas)
            .Include(item => item.CatalogoRepuestos)
            .FirstOrDefaultAsync(item => item.Id == proveedor.Id && item.Existe);

        if (existente is null)
        {
            return false;
        }

        CopiarDatos(proveedor, existente);
        SincronizarRepuestosOfrecidos(contexto, existente, existente.CatalogoRepuestos, catalogos.RepuestosOfrecidos);
        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();
        return true;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var proveedor = await contexto.Proveedores
            .FirstOrDefaultAsync(item => item.Id == id && item.Existe);

        if (proveedor is null)
        {
            return false;
        }

        proveedor.Existe = false;
        await contexto.SaveChangesAsync();
        return true;
    }

    private static void CopiarDatos(Proveedores origen, Proveedores destino)
    {
        destino.Nombre = origen.Nombre;
        destino.Documento = origen.Documento;
        destino.Telefono = origen.Telefono;
        destino.Correo = origen.Correo;
        destino.Ciudad = origen.Ciudad;
        destino.Estado = origen.Estado;
        destino.Direccion = origen.Direccion;
        destino.TipoProveedor = origen.TipoProveedor;
        destino.NombreContacto = origen.NombreContacto;
        destino.PaginaWeb = origen.PaginaWeb;
        destino.Pais = origen.Pais;
        destino.Provincia = origen.Provincia;
        destino.TerminosPago = origen.TerminosPago;
        destino.LimiteCredito = origen.LimiteCredito;
        destino.TiempoEntregaDias = origen.TiempoEntregaDias;
        destino.GarantiaOfrecida = origen.GarantiaOfrecida;
        destino.PoliticaDevolucion = origen.PoliticaDevolucion;
        destino.Observaciones = origen.Observaciones;
        destino.TipoVehiculos = origen.TipoVehiculos;
        destino.PaisProcedencia = origen.PaisProcedencia;
        destino.TipoPiezas = origen.TipoPiezas;
        destino.TipoServicios = origen.TipoServicios;
        destino.Especialidad = origen.Especialidad;
        destino.ZonaCobertura = origen.ZonaCobertura;
    }

    private static async Task SincronizarCatalogos(
        Contexto contexto,
        Proveedores proveedor,
        ProveedoresCatalogos catalogos)
    {
        var necesitaVehiculos = proveedor.TipoProveedor is TiposProveedor.Vehiculos or TiposProveedor.VehiculosYPiezas;
        var necesitaProductos = proveedor.TipoProveedor is TiposProveedor.PiezasYRepuestos
            or TiposProveedor.VehiculosYPiezas
            or TiposProveedor.LubricantesYAccesorios;
        var marcas = new List<MarcaProveedor>();
        var modelos = new List<ModeloVehiculoProveedor>();
        var categorias = new List<CategoriaPiezaProveedor>();

        if (necesitaVehiculos || necesitaProductos)
        {
            foreach (var nombre in catalogos.Marcas)
            {
                var marca = await ObtenerOCrear(
                    contexto.MarcasProveedor,
                    nombre,
                    item => item.Nombre,
                    nombreNuevo => new MarcaProveedor { Nombre = nombreNuevo });
                marcas.Add(marca);
            }
        }

        if (necesitaVehiculos)
        {
            foreach (var nombre in catalogos.Modelos)
            {
                var modelo = await ObtenerOCrear(
                    contexto.ModelosVehiculosProveedor,
                    nombre,
                    item => item.Nombre,
                    nombreNuevo => new ModeloVehiculoProveedor { Nombre = nombreNuevo });
                modelos.Add(modelo);
            }
        }

        if (necesitaProductos)
        {
            foreach (var nombre in catalogos.Categorias)
            {
                var categoria = await ObtenerOCrear(
                    contexto.CategoriasPiezasProveedor,
                    nombre,
                    item => item.Nombre,
                    nombreNuevo => new CategoriaPiezaProveedor { Nombre = nombreNuevo });
                categorias.Add(categoria);
            }
        }

        await contexto.SaveChangesAsync();

        SincronizarRelaciones(
            contexto.ProveedoresMarcas,
            proveedor.Marcas,
            marcas,
            relacion => relacion.MarcaId,
            marca => marca.Id,
            marca => new ProveedorMarca { Proveedor = proveedor, Marca = marca });
        SincronizarRelaciones(
            contexto.ProveedoresModelosVehiculos,
            proveedor.ModelosVehiculos,
            modelos,
            relacion => relacion.ModeloVehiculoId,
            modelo => modelo.Id,
            modelo => new ProveedorModeloVehiculo { Proveedor = proveedor, ModeloVehiculo = modelo });
        SincronizarRelaciones(
            contexto.ProveedoresCategoriasPiezas,
            proveedor.CategoriasPiezas,
            categorias,
            relacion => relacion.CategoriaPiezaId,
            categoria => categoria.Id,
            categoria => new ProveedorCategoriaPieza { Proveedor = proveedor, CategoriaPieza = categoria });
    }

    private static void SincronizarRelaciones<TRelacion, TCatalogo>(
        DbSet<TRelacion> dbSet,
        ICollection<TRelacion> relacionesExistentes,
        IReadOnlyCollection<TCatalogo> catalogosDeseados,
        Func<TRelacion, int> obtenerIdRelacion,
        Func<TCatalogo, int> obtenerIdCatalogo,
        Func<TCatalogo, TRelacion> crearRelacion)
        where TRelacion : class
    {
        var idsDeseados = catalogosDeseados.Select(obtenerIdCatalogo).ToHashSet();
        var relacionesRemovidas = relacionesExistentes
            .Where(relacion => !idsDeseados.Contains(obtenerIdRelacion(relacion)))
            .ToList();
        dbSet.RemoveRange(relacionesRemovidas);

        foreach (var catalogo in catalogosDeseados)
        {
            if (!relacionesExistentes.Any(relacion => obtenerIdRelacion(relacion) == obtenerIdCatalogo(catalogo)))
            {
                relacionesExistentes.Add(crearRelacion(catalogo));
            }
        }
    }

    private static void SincronizarRepuestosOfrecidos(
        Contexto contexto,
        Proveedores proveedor,
        ICollection<RepuestoOfertaProveedor> ofertasExistentes,
        IReadOnlyCollection<RepuestoOfertaProveedor> ofertasEnviadas)
    {
        var ofertasConId = ofertasEnviadas.Where(oferta => oferta.Id > 0).ToList();
        if (ofertasConId.Select(oferta => oferta.Id).Distinct().Count() != ofertasConId.Count)
        {
            throw new InvalidOperationException("El catálogo contiene repuestos duplicados.");
        }

        var idsExistentes = ofertasExistentes.Select(oferta => oferta.Id).ToHashSet();
        if (ofertasConId.Any(oferta => !idsExistentes.Contains(oferta.Id)))
        {
            throw new InvalidOperationException("No se puede asociar un repuesto del catálogo de otro proveedor.");
        }

        var idsConservados = ofertasConId.Select(oferta => oferta.Id).ToHashSet();
        var ofertasEliminadas = ofertasExistentes
            .Where(oferta => !idsConservados.Contains(oferta.Id))
            .ToList();
        contexto.RepuestosOfrecidosProveedor.RemoveRange(ofertasEliminadas);
        foreach (var oferta in ofertasEliminadas)
        {
            ofertasExistentes.Remove(oferta);
        }

        foreach (var ofertaEnviada in ofertasEnviadas)
        {
            if (ofertaEnviada.Id == 0)
            {
                proveedor.CatalogoRepuestos.Add(new RepuestoOfertaProveedor
                {
                    Proveedor = proveedor,
                    Nombre = ofertaEnviada.Nombre,
                    Marca = ofertaEnviada.Marca,
                    Tipo = ofertaEnviada.Tipo,
                    Tamano = ofertaEnviada.Tamano,
                    Cantidad = ofertaEnviada.Cantidad,
                    Precio = ofertaEnviada.Precio
                });
                continue;
            }

            var existente = ofertasExistentes.First(oferta => oferta.Id == ofertaEnviada.Id);
            existente.Nombre = ofertaEnviada.Nombre;
            existente.Marca = ofertaEnviada.Marca;
            existente.Tipo = ofertaEnviada.Tipo;
            existente.Tamano = ofertaEnviada.Tamano;
            existente.Cantidad = ofertaEnviada.Cantidad;
            existente.Precio = ofertaEnviada.Precio;
        }
    }

    private static async Task<TCatalogo> ObtenerOCrear<TCatalogo>(
        DbSet<TCatalogo> dbSet,
        string nombre,
        System.Linq.Expressions.Expression<Func<TCatalogo, string>> selector,
        Func<string, TCatalogo> crear)
        where TCatalogo : class
    {
        var propiedad = selector.Compile();
        var existente = dbSet.Local.FirstOrDefault(
            item => string.Equals(propiedad(item), nombre, StringComparison.OrdinalIgnoreCase));
        if (existente is null)
        {
            var filtro = Expression.Lambda<Func<TCatalogo, bool>>(
                Expression.Equal(selector.Body, Expression.Constant(nombre)),
                selector.Parameters);
            existente = await dbSet.FirstOrDefaultAsync(filtro);
        }

        if (existente is not null)
        {
            return existente;
        }

        var nuevo = crear(nombre);
        dbSet.Add(nuevo);
        return nuevo;
    }

    private static void Validar(Proveedores proveedor)
    {
        proveedor.Nombre = Normalizar(proveedor.Nombre);
        proveedor.Documento = Normalizar(proveedor.Documento);
        proveedor.Telefono = Normalizar(proveedor.Telefono);
        proveedor.Correo = Normalizar(proveedor.Correo);
        proveedor.Ciudad = Normalizar(proveedor.Ciudad);
        proveedor.Estado = Normalizar(proveedor.Estado);
        proveedor.Direccion = Normalizar(proveedor.Direccion);
        proveedor.TipoProveedor = Normalizar(proveedor.TipoProveedor);
        proveedor.NombreContacto = Normalizar(proveedor.NombreContacto);
        proveedor.PaginaWeb = Normalizar(proveedor.PaginaWeb);
        proveedor.Pais = Normalizar(proveedor.Pais);
        proveedor.Provincia = Normalizar(proveedor.Provincia);
        proveedor.TerminosPago = Normalizar(proveedor.TerminosPago);
        proveedor.GarantiaOfrecida = Normalizar(proveedor.GarantiaOfrecida);
        proveedor.PoliticaDevolucion = Normalizar(proveedor.PoliticaDevolucion);
        proveedor.Observaciones = Normalizar(proveedor.Observaciones);
        proveedor.TipoVehiculos = Normalizar(proveedor.TipoVehiculos);
        proveedor.PaisProcedencia = Normalizar(proveedor.PaisProcedencia);
        proveedor.TipoPiezas = Normalizar(proveedor.TipoPiezas);
        proveedor.TipoServicios = Normalizar(proveedor.TipoServicios);
        proveedor.Especialidad = Normalizar(proveedor.Especialidad);
        proveedor.ZonaCobertura = Normalizar(proveedor.ZonaCobertura);

        if (!TiposProveedor.Opciones.Contains(proveedor.TipoProveedor))
        {
            proveedor.TipoProveedor = TiposProveedor.PiezasYRepuestos;
        }

        if (!TerminosPagoValidos.Contains(proveedor.TerminosPago))
        {
            proveedor.TerminosPago = "Contado";
        }

        var errores = new List<ValidationResult>();
        var contextoValidacion = new ValidationContext(proveedor);
        if (!Validator.TryValidateObject(proveedor, contextoValidacion, errores, validateAllProperties: true))
        {
            throw new InvalidOperationException(string.Join(" ", errores.Select(error => error.ErrorMessage)));
        }

        if (proveedor.Estado is not ("Activo" or "Inactivo"))
        {
            throw new InvalidOperationException("El estado del proveedor debe ser Activo o Inactivo.");
        }

    }

    private static ProveedoresCatalogos NormalizarCatalogos(ProveedoresCatalogos catalogos) =>
        new(
            catalogos.Marcas.Select(Normalizar).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            catalogos.Modelos.Select(Normalizar).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            catalogos.Categorias.Select(Normalizar).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            catalogos.RepuestosOfrecidos.Select(oferta => new RepuestoOfertaProveedor
            {
                Id = oferta.Id,
                Nombre = Normalizar(oferta.Nombre),
                Marca = Normalizar(oferta.Marca),
                Tipo = Normalizar(oferta.Tipo),
                Tamano = string.IsNullOrWhiteSpace(oferta.Tamano) ? null : oferta.Tamano.Trim(),
                Cantidad = oferta.Cantidad,
                Precio = oferta.Precio
            }).ToArray());

    private static void ValidarCatalogos(ProveedoresCatalogos catalogos)
    {
        ValidarNombres(catalogos.Marcas, 80, "marca");
        ValidarNombres(catalogos.Modelos, 100, "modelo");
        ValidarNombres(catalogos.Categorias, 100, "categoría");

        if (catalogos.RepuestosOfrecidos.Count > 500)
        {
            throw new InvalidOperationException("No se permiten más de 500 repuestos en el catálogo del proveedor.");
        }

        var errores = catalogos.RepuestosOfrecidos
            .SelectMany(oferta =>
            {
                var resultados = new List<ValidationResult>();
                Validator.TryValidateObject(oferta, new ValidationContext(oferta), resultados, validateAllProperties: true);
                return resultados;
            })
            .ToList();
        if (errores.Count > 0)
        {
            throw new InvalidOperationException(string.Join(" ", errores.Select(error => error.ErrorMessage)));
        }
    }

    private static void ValidarNombres(IEnumerable<string> nombres, int maxLength, string tipo)
    {
        var lista = nombres.ToList();
        if (lista.Count > 50)
        {
            throw new InvalidOperationException($"No se permiten más de 50 valores para {tipo}s.");
        }

        foreach (var nombre in lista)
        {
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > maxLength)
            {
                throw new InvalidOperationException($"Cada {tipo} debe tener entre 1 y {maxLength} caracteres.");
            }
        }
    }

    private static string Normalizar(string? valor) => valor?.Trim() ?? string.Empty;
}
