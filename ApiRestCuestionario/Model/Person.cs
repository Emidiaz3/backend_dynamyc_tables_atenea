using System.ComponentModel.DataAnnotations;
using System;

namespace ApiRestCuestionario.Model
{
    public class Persona
    {
        [Key]
        public int? Id { get; set; }
        public string Id_Persona { get; set; }
        public string? NomPersona { get; set; }
        public int? Proyecto { get; set; }
        public int? Localidad { get; set; }
        public string? Nivel_Riesgo_General { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string? TipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public int? Id_Ref { get; set; }
    }
}
