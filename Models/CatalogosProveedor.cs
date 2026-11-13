using System.ComponentModel.DataAnnotations;

namespace DLeon_Asociados_Web.Models;

public class MarcaProveedor
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<ProveedorMarca> Proveedores { get; set; } = new List<ProveedorMarca>();
}

public class ModeloVehiculoProveedor
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<ProveedorModeloVehiculo> Proveedores { get; set; } = new List<ProveedorModeloVehiculo>();
}

public class CategoriaPiezaProveedor
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<ProveedorCategoriaPieza> Proveedores { get; set; } = new List<ProveedorCategoriaPieza>();
}

public class ProveedorMarca
{
    public int ProveedorId { get; set; }
    public Proveedores Proveedor { get; set; } = null!;

    public int MarcaId { get; set; }
    public MarcaProveedor Marca { get; set; } = null!;
}

public class ProveedorModeloVehiculo
{
    public int ProveedorId { get; set; }
    public Proveedores Proveedor { get; set; } = null!;

    public int ModeloVehiculoId { get; set; }
    public ModeloVehiculoProveedor ModeloVehiculo { get; set; } = null!;
}

public class ProveedorCategoriaPieza
{
    public int ProveedorId { get; set; }
    public Proveedores Proveedor { get; set; } = null!;

    public int CategoriaPiezaId { get; set; }
    public CategoriaPiezaProveedor CategoriaPieza { get; set; } = null!;
}

public class RepuestoOfertaProveedor
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }
    public Proveedores Proveedor { get; set; } = null!;

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Tipo { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Tamano { get; set; }

    [Range(0, int.MaxValue)]
    public int Cantidad { get; set; }

    [Range(typeof(decimal), "0", "100000000")]
    public decimal Precio { get; set; }
}
