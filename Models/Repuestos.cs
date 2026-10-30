using System.ComponentModel.DataAnnotations;

namespace DLeon_Asociados_Web.Models;

public class Repuestos
{
    [Key]
    public int Id { get; set; }

    public bool Existe { get; set; } = true;

    [Required]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Tipo { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Tamano { get; set; }

    [Range(0, int.MaxValue)]
    public int Disponibles { get; set; }

    [Range(typeof(decimal), "0", "100000000")]
    public decimal PrecioUnitario { get; set; }

    [StringLength(120)]
    public string? Proveedor { get; set; }
}
