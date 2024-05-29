using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{
    public class Organizacion_localidad
    {
        [Key]
        public int idOrganizacionLocalidad { get; set; }
        public int? idOrganizacion { get; set; }
        public int idLocalidad { get; set; }
        public int idusuario { get; set; }
    }
}
