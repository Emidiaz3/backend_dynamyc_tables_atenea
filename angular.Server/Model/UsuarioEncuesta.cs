using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestCuestionario.Model
{
    public class UsuarioEncuesta
    {

        public int Id { get; set; }
        public string? Form_name { get; set; }
        public int Form_id { get; set; }
        public int Users_id { get; set; }
        public string? Link { get; set; }
        public int? IdTipoEncuesta { get; set; }
        public int IdProyecto { get; set; }
    }
}
