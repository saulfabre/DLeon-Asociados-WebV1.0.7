using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLeon_Asociados_Web.Models;

public class Seguros
{
    [Key]
    public int Id { get; set; }

    public bool Existe { get; set; } = true;

    [Required]
    [Range(1, int.MaxValue)]
    public int VehiculoId { get; set; }

    public Vehiculos? Vehiculo { get; set; }

    public int? ClienteId { get; set; }

    public Clientes? Cliente { get; set; }

    [Required]
    [StringLength(80)]
    public string ClienteNombre { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ClienteApellido { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string ClienteDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string ClienteTelefono { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [EmailAddress]
    public string ClienteCorreo { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string ClienteDireccion { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string NumeroSeguro { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string TipoProveedor { get; set; } = "Propio";

    [Required]
    [StringLength(120)]
    public string ProveedorNombre { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Tipo { get; set; } = "Estandar";

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaExpiracion { get; set; } = DateTime.Today.AddYears(1);

    [NotMapped]
    public int DiasRestantes => (FechaExpiracion.Date - DateTime.Today).Days;

    [NotMapped]
    public string Estado => DiasRestantes < 0
        ? "Vencido"
        : DiasRestantes <= 30
            ? "Expira pronto"
            : "Activo";
}