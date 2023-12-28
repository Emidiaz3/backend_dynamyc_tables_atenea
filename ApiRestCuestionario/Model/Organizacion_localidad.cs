using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Organizacion_localidad
    {
        public Organizacion_localidad() { }
        [Key]
        public int idOrganizacionLocalidad { get; set; }
        public int? idOrganizacion { get; set; }
        public int idLocalidad { get; set; }
        public int idusuario { get; set; }
    }
}
