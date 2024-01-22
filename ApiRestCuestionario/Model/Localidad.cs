using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Localidad
    {
        [Key]
        public int? Id { get; set; }
        public string Id_Localidad { get; set; }
        public string NomLocalidad { get; set; }
        public int? Proyecto { get; set; }
        public int? Departamento { get; set; }
        public int? Provincia { get; set; }
        public int? Distrito { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public int? Id_Ref { get; set; }
    }
}
