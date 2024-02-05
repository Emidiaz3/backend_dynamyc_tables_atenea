using System;
using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{
    public class Localidad
    {
        [Key]
        public int? IdLocalidad { get; set; }
        public string CodigoLocalidad { get; set; }
        public string NomLocalidad { get; set; }
        public int? IdProyecto { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdRef { get; set; }
    }
}
