using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLeon_Asociados_Web.Models;

public class Garantias
{
    [Key]
    public int Id { get; set; }

    public bool Existe { get; set; } = true;

    [Required]
    [Range(1, int.MaxValue)]
    public int VehiculoId { get; set; }

    public Vehiculos? Vehiculo { get; set; }

    [Required]
    [StringLength(100)]
    public string Tipo { get; set; } = string.Empty;

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
        ? "Vencida"
        : DiasRestantes <= 30
            ? "Expira pronto"
            : "Vigente";
}
