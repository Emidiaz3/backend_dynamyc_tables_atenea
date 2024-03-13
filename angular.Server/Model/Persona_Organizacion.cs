using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{
    public class Persona_Organizacion
    {
        [Key]
        public int? Id { get; set; }
        public int? IdPersona { get; set; }
        public int? IdOrganizacion { get; set; }
    }
}
