using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Linq;


namespace MinimalApi.Models;

public partial class ClientesPruebaTecnicaContext : DbContext
{
    public ClientesPruebaTecnicaContext()
    {
    }

    public ClientesPruebaTecnicaContext(DbContextOptions<ClientesPruebaTecnicaContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<TiposDocumento> TiposDocumento { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci").HasCharSet("utf8mb4");

        ConfigureCliente(modelBuilder);
        ConfigureTiposDocumento(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    private void ConfigureCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PRIMARY");
            entity.ToTable("clientes");

            entity.HasIndex(e => e.TipoDocumentoId, "tipo_documento");

            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Apellidos).HasMaxLength(50).HasColumnName("apellidos");
            entity.Property(e => e.CorreoElectronico).HasMaxLength(100).HasColumnName("correo_electronico");
            entity.Property(e => e.Departamento).HasMaxLength(50).HasColumnName("departamento");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.NumeroDocumento).HasMaxLength(20).HasColumnName("numero_documento");
            entity.Property(e => e.NumeroTelefono).HasMaxLength(20).HasColumnName("numero_telefono");
            entity.Property(e => e.Pais).HasMaxLength(50).HasColumnName("pais");
            entity.Property(e => e.TipoDocumentoId).HasColumnName("tipo_documento");

            entity.HasOne(d => d.TipoDocumento).WithMany(p => p.Clientes)
                  .HasForeignKey(d => d.TipoDocumentoId)
                  .HasConstraintName("clientes_ibfk_1");
        });
    }

    private void ConfigureTiposDocumento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TiposDocumento>(entity =>
        {
            entity.HasKey(e => e.TipoDocumentoId).HasName("PRIMARY");
            entity.ToTable("tipos_documento");

            entity.Property(e => e.TipoDocumentoId).HasColumnName("tipo_documento_id");
            entity.Property(e => e.TipoDocumentoNombre).HasMaxLength(50).HasColumnName("tipo_documento_nombre");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public IQueryable<ClienteConTipoDocumento> GetClientesConTipoDocumento(ClientesPruebaTecnicaContext context)
    {
        var clientesConTipoDocumento = from cliente in context.Clientes
                                       join tipoDocumento in context.TiposDocumento
                                       on cliente.TipoDocumentoId equals tipoDocumento.TipoDocumentoId
                                       select new ClienteConTipoDocumento
                                       {
                                           ClienteId = cliente.ClienteId,
                                           Nombre = cliente.Nombre,
                                           Apellidos = cliente.Apellidos,
                                           TipoDocumentoNombre = tipoDocumento.TipoDocumentoNombre,
                                           TipoDocumentoId = cliente.TipoDocumentoId,
                                           NumeroDocumento = cliente.NumeroDocumento,
                                           Pais = cliente.Pais,
                                           Departamento = cliente.Departamento,
                                           NumeroTelefono = cliente.NumeroTelefono,
                                           CorreoElectronico = cliente.CorreoElectronico,
                                           FechaNacimiento = cliente.FechaNacimiento
                                       };

        return clientesConTipoDocumento;
    }

    public async Task<Dictionary<string, int>> GetDocumentCountsForTiposDocumentoAsync()
    {
        var tiposDocumento = await TiposDocumento.ToListAsync();

        var counts = new Dictionary<string, int>();

        foreach (var tipoDocumento in tiposDocumento)
        {
            var count = await Clientes
                .Where(cliente => cliente.TipoDocumentoId == tipoDocumento.TipoDocumentoId)
                .CountAsync();

            counts.Add(tipoDocumento.TipoDocumentoNombre, count);
        }

        return counts;
    }
}
