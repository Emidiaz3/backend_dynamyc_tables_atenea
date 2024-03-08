namespace ApiRestCuestionario.Model
{
    public class UsuarioEncuesta
    {
        public int? id { get; set; }
        public string? form_name { get; set; }
        public int? form_id { get; set; }
        public int? users_id { get; set; }
        public string? link { get; set; }
        public int? idTipoEncuesta { get; set; }
        public int? idProyecto { get; set; }
    }
}
