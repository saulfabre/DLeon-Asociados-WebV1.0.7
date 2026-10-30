namespace DLeon_Asociados_Web.Services;

public sealed class ProveedoresStore
{
    private readonly List<ProveedorRegistro> proveedores = new()
    {
        new("Repuestos Saul RS", "056-548741-4", "809-876-6412", "Saul12@gmail.com", "Santo Domingo", "Activo", ""),
        new("Distribuidor Euwin.BR", "045-458741-1", "809-721-9539", "Euwin@gmail.com", "Puerto Plata", "Inactivo", ""),
        new("Repuesto Esthaly.ER", "408-87475-8", "829-987-8745", "Esthaly@gmail.com", "Tenares", "Activo", ""),
        new("Repuesto Lola.rd", "098-871857-3", "849-245-8745", "Lola@gmail.com", "Jarabacoa", "Inactivo", ""),
        new("Importadora Juan.GD", "056-878451-8", "809-874-5698", "Juan@gmail.com", "San Francisco de Macorís", "Activo", "")
    };

    public IReadOnlyList<ProveedorRegistro> ObtenerTodos() => proveedores;

    public void Agregar(ProveedorRegistro proveedor) => proveedores.Add(proveedor);

    public sealed record ProveedorRegistro(
        string Nombre,
        string Documento,
        string Telefono,
        string Correo,
        string Ciudad,
        string Estado,
        string Direccion);
}
