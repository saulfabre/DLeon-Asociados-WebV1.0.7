using System.ComponentModel.DataAnnotations;

namespace DLeon_Asociados_Web.Models;

public class Proveedores
{
    [Key]
    public int Id { get; set; }
    public bool Existe { get; set; } = true;
    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;
    [Required, StringLength(40)]
    public string Documento { get; set; } = string.Empty;
    [Required, StringLength(30)]
    public string Telefono { get; set; } = string.Empty;
    [StringLength(150), EmailAddress]
    public string Correo { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string Ciudad { get; set; } = string.Empty;
    [StringLength(20)]
    public string Estado { get; set; } = "Activo";
    [StringLength(250)]
    public string Direccion { get; set; } = string.Empty;

    [StringLength(50)]
    public string TipoProveedor { get; set; } = string.Empty;
    [StringLength(120)]
    public string NombreContacto { get; set; } = string.Empty;
    [StringLength(200)]
    public string PaginaWeb { get; set; } = string.Empty;
    [StringLength(80)]
    public string Pais { get; set; } = string.Empty;
    [StringLength(100)]
    public string Provincia { get; set; } = string.Empty;
    [StringLength(30)]
    public string TerminosPago { get; set; } = "Contado";
    [Range(typeof(decimal), "0", "999999999999")]
    public decimal LimiteCredito { get; set; }
    [Range(0, 3650)]
    public int? TiempoEntregaDias { get; set; }
    [StringLength(1000)]
    public string GarantiaOfrecida { get; set; } = string.Empty;
    [StringLength(1000)]
    public string PoliticaDevolucion { get; set; } = string.Empty;
    [StringLength(2000)]
    public string Observaciones { get; set; } = string.Empty;

    [StringLength(20)]
    public string TipoVehiculos { get; set; } = string.Empty;
    [StringLength(80)]
    public string PaisProcedencia { get; set; } = string.Empty;
    [StringLength(30)]
    public string TipoPiezas { get; set; } = string.Empty;
    [StringLength(120)]
    public string TipoServicios { get; set; } = string.Empty;
    [StringLength(120)]
    public string Especialidad { get; set; } = string.Empty;
    [StringLength(200)]
    public string ZonaCobertura { get; set; } = string.Empty;

    public ICollection<ProveedorMarca> Marcas { get; set; } = new List<ProveedorMarca>();
    public ICollection<ProveedorModeloVehiculo> ModelosVehiculos { get; set; } = new List<ProveedorModeloVehiculo>();
    public ICollection<ProveedorCategoriaPieza> CategoriasPiezas { get; set; } = new List<ProveedorCategoriaPieza>();
    public ICollection<Repuestos> Repuestos { get; set; } = new List<Repuestos>();
    public ICollection<RepuestoOfertaProveedor> CatalogoRepuestos { get; set; } = new List<RepuestoOfertaProveedor>();
}

public static class TiposProveedor
{
    public const string Vehiculos = "Vehículos";
    public const string PiezasYRepuestos = "Piezas y repuestos";
    public const string VehiculosYPiezas = "Vehículos y piezas";
    public const string ServiciosAutomotrices = "Servicios automotrices";
    public const string LubricantesYAccesorios = "Lubricantes y accesorios";

    public static IReadOnlyList<string> Opciones { get; } =
    [
        Vehiculos,
        PiezasYRepuestos,
        VehiculosYPiezas,
        ServiciosAutomotrices,
        LubricantesYAccesorios
    ];
}
