using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DLeon_Asociados_Web.Models;

public class Clientes
{
    [Key]
    [Column("IdCliente")]
    public int Id { get; set; }

    public bool Existe { get; set; } = true;

    [StringLength(160)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [NotMapped]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [NotMapped]
    public string Apellido { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string? Documento { get; set; }

    [Required, StringLength(30)]
    public string? Telefono { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Correo { get; set; }

    [StringLength(250)]
    public string? Direccion { get; set; }

    public ICollection<Seguros> Seguros { get; set; } = new List<Seguros>();
}