using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinimalApi.Models;

public partial class TiposDocumento
{
    [Key]
    [Column("tipo_documento_id")]
    public int TipoDocumentoId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo_documento_nombre")]
    public string? TipoDocumentoNombre { get; set; }

    [JsonIgnore]
    public ICollection<Cliente> Clientes { get; set; }
}
