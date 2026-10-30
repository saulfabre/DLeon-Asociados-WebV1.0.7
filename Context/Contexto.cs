using DLeon_Asociados_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DLeon_Asociados_Web.Context;

public class Contexto : DbContext
{
	public Contexto(DbContextOptions<Contexto> options)
		: base(options)
	{
	}

	public DbSet<Vehiculos> Vehiculos { get; set; }
	public DbSet<Seguros> Seguros { get; set; }
	public DbSet<Clientes> Clientes { get; set; }
	public DbSet<Garantias> Garantias { get; set; }
	public DbSet<Repuestos> Repuestos { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Vehiculos>(entity =>
		{
			entity.HasKey(v => v.Id);
			entity.Property(v => v.Marca).HasMaxLength(80).IsRequired();
			entity.Property(v => v.Modelo).HasMaxLength(80).IsRequired();
			entity.Property(v => v.Matricula).HasMaxLength(20).IsRequired();
			entity.Property(v => v.Tipo).HasMaxLength(80).IsRequired();
			entity.Property(v => v.Nombre).HasMaxLength(200).IsRequired();
			entity.Property(v => v.Existe).HasDefaultValue(true);
			entity.Property(v => v.Precio).HasColumnType("decimal(18,2)");
			entity.HasIndex(v => new { v.Marca, v.Modelo });
			entity.HasIndex(v => v.Estado);
		});

		modelBuilder.Entity<Clientes>(entity =>
		{
			entity.HasKey(c => c.Id);
			entity.Property(c => c.Id).HasColumnName("IdCliente");
			entity.Property(c => c.Existe).HasDefaultValue(true);
			entity.Property(c => c.NombreCompleto).HasColumnName("NombreCompleto").HasMaxLength(160).IsUnicode(false).IsRequired();
			entity.Property(c => c.Documento).HasMaxLength(40).IsUnicode(false).IsRequired(false);
			entity.Property(c => c.Telefono).HasMaxLength(30).IsUnicode(false).IsRequired(false);
			entity.Property(c => c.Correo).HasMaxLength(150).IsUnicode(false).IsRequired(false);
			entity.Property(c => c.Direccion).HasMaxLength(250).IsUnicode(false).IsRequired(false);
			entity.HasIndex(c => c.Documento).IsUnique().HasFilter("[Documento] IS NOT NULL AND [Existe] = 1");
		});

		modelBuilder.Entity<Seguros>(entity =>
		{
			entity.HasKey(s => s.Id);
			entity.Property(s => s.Existe).HasDefaultValue(true);
			entity.Property(s => s.Tipo).HasMaxLength(80).IsRequired();
			entity.Property(s => s.NumeroSeguro).HasMaxLength(50).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.TipoProveedor).HasMaxLength(20).HasDefaultValue("Propio").IsRequired();
			entity.Property(s => s.ProveedorNombre).HasMaxLength(120).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteNombre).HasMaxLength(80).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteApellido).HasMaxLength(80).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteDocumento).HasMaxLength(40).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteTelefono).HasMaxLength(30).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteCorreo).HasMaxLength(150).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.ClienteDireccion).HasMaxLength(250).HasDefaultValue(string.Empty).IsRequired();
			entity.Property(s => s.FechaInicio).HasColumnType("date");
			entity.Property(s => s.FechaExpiracion).HasColumnType("date");
			entity.HasIndex(s => s.VehiculoId);
			entity.HasIndex(s => s.ClienteId);
			entity.HasIndex(s => s.FechaExpiracion);
			entity.HasOne(s => s.Vehiculo)
				.WithMany(v => v.Seguros)
				.HasForeignKey(s => s.VehiculoId)
				.OnDelete(DeleteBehavior.Restrict);
			entity.HasOne(s => s.Cliente)
				.WithMany(c => c.Seguros)
				.HasForeignKey(s => s.ClienteId)
				.OnDelete(DeleteBehavior.SetNull);
		});

		modelBuilder.Entity<Garantias>(entity =>
		{
			entity.HasKey(g => g.Id);
			entity.Property(g => g.Existe).HasDefaultValue(true);
			entity.Property(g => g.Tipo).HasMaxLength(100).IsRequired();
			entity.Property(g => g.FechaInicio).HasColumnType("date");
			entity.Property(g => g.FechaExpiracion).HasColumnType("date");
			entity.HasIndex(g => g.VehiculoId);
			entity.HasIndex(g => g.FechaExpiracion);
			entity.HasOne(g => g.Vehiculo)
				.WithMany()
				.HasForeignKey(g => g.VehiculoId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Repuestos>(entity =>
		{
			entity.HasKey(repuesto => repuesto.Id);
			entity.Property(repuesto => repuesto.Existe).HasDefaultValue(true);
			entity.Property(repuesto => repuesto.Nombre).HasMaxLength(120).IsRequired();
			entity.Property(repuesto => repuesto.Marca).HasMaxLength(80).IsRequired();
			entity.Property(repuesto => repuesto.Tipo).HasMaxLength(80).IsRequired();
			entity.Property(repuesto => repuesto.Tamano).HasMaxLength(120);
			entity.Property(repuesto => repuesto.PrecioUnitario).HasColumnType("decimal(18,2)");
			entity.Property(repuesto => repuesto.Proveedor).HasMaxLength(120);
		});
	}
}
