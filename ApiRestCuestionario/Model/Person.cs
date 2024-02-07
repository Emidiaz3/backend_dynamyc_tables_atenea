using System.ComponentModel.DataAnnotations;
using System;

namespace ApiRestCuestionario.Model
{
    public class Persona
    {
        [Key]
        public int? IdPersona { get; set; }
        public string? CodigoPersona { get; set; }
        public string? NomPersona { get; set; }
        public int? IdProyecto { get; set; }
        public int? IdLocalidad { get; set; }
        public string? NivelRiesgoGeneral { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string? TipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdRef { get; set; }
    }
}
