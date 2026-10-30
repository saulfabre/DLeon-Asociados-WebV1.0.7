using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLeon_Asociados_Web.Models;

public class Vehiculos
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Modelo { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Matricula { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Edicion { get; set; }

    [Required]
    [StringLength(80)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public int Anio { get; set; }

    public bool Existe { get; set; } = true;

    [Required]
    [Range(0, 100000000)]
    public decimal Precio { get; set; }

    [StringLength(30)]
    public string Estado { get; set; } = "Disponible";

    [StringLength(30)]
    public string? Color { get; set; }

    [StringLength(30)]
    public string? Combustible { get; set; }

    [StringLength(30)]
    public string? Transmision { get; set; }

    public int? Kilometraje { get; set; }

    [StringLength(2000)]
    public string? Descripcion { get; set; }

    [StringLength(500)]
    public string? Imagen { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    public DateTime? FechaPublicacion { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Seguros> Seguros { get; set; } = new List<Seguros>();

    [NotMapped]
    public string Millaje => Kilometraje.HasValue ? $"{Kilometraje.Value:N0} Km" : "Sin registro";

    [NotMapped]
    public string NombreCompleto => !string.IsNullOrWhiteSpace(Nombre)
        ? Nombre
        : string.Join(' ', new[] { Marca, Modelo, Edicion }.Where(value => !string.IsNullOrWhiteSpace(value)));
}
