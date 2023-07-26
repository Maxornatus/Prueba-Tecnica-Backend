using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinimalApi.Models;

public class Cliente
{
    [Key]
    [Column("cliente_id")]
    public int ClienteId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nombre")]
    public string Nombre { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("apellidos")]
    public string Apellidos { get; set; }

    [Column("tipo_documento_id")]
    public int? TipoDocumentoId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("numero_documento")]
    public string NumeroDocumento { get; set; }

    [MaxLength(50)]
    [Column("pais")]
    public string Pais { get; set; }

    [MaxLength(50)]
    [Column("departamento")]
    public string Departamento { get; set; }

    [MaxLength(20)]
    [Column("numero_telefono")]
    public string NumeroTelefono { get; set; }

    [MaxLength(100)]
    [Column("correo_electronico")]
    public string CorreoElectronico { get; set; }

    [Column("fecha_nacimiento")]
    public DateTime? FechaNacimiento { get; set; }

    // Propiedad de navegación para la entidad relacionada "TipoDocumento"
    [ForeignKey("TipoDocumentoId")]
    [JsonIgnore]
    public TiposDocumento TipoDocumento { get; set; }
}
