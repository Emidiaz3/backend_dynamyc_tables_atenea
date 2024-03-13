using System;
using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{
    public class Organizacion
    {
        public Organizacion() { }

        [Key]
        public int? IdOrganizacion { get; set; }
        public string CodigoOrganizacion { get; set; }
        public string NomOrganizacion { get; set; }
        public int? IdProyecto { get; set; }
        public int? IdLocalidad { get; set; }
        public string NivelRiesgoGeneral { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdRef { get; set; }

    }
}
