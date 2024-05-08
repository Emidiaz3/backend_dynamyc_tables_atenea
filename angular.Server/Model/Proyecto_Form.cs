using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{
    public class Proyecto_Form
    {
        [Key]
        public int IdProyecto { get; set; }
        public int IdEmpresa { get; set; }
        public string? DescripcionEmpresa { get; set; }
        public string? NombreProyecto { get; set; }
        public int? EstadoProyecto { get; set; }
        public int Flg_Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaAccion { get; set; }
        public int? UsuarioAccion { get; set; }
        public string? NombreDB { get; set; }
    }

    public class entidad_guardar_proyecto
    {
        public int IdProyecto { get; set; }
        public string? NombreProyecto { get; set; }
        public string? NombreDB { get; set; }
        public int IdEmpresa { get; set; }
        public int EstadoProyecto { get; set; }
        public int? UsuarioAccion { get; set; }
    }

    public class lst_usuarios_proyectos
    {
        [Key]
        public int IdRelUsuProy { get; set; }
        public int IdProyecto { get; set; }
        public string? NombreProyecto { get; set; }
        public int IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public int Flg_Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaAccion { get; set; }
        public int? UsuarioAccion { get; set; }
    }

    public class entidad_guardar_proyecto_usuario
    {
        public int IdRelUsuProy { get; set; }
        public int IdProyecto { get; set; }
        public int IdUsuario { get; set; }
        public int? UsuarioAccion { get; set; }
    }
    public class entity_delete_proy
    {
        public int IdProyecto { get; set; }
        public int? UsuarioAccion { get; set; }
    }
    public class entity_delete_proy_usu
    {
        public int IdRelUsuProy { get; set; }
        public int? UsuarioAccion { get; set; }
    }

    public class entity_get_proys
    {
        [Key]
        public int IdProyecto { get; set; }
        public string? NombreProyecto { get; set; }
        public string? NombreDB { get; set; }
    }

    /*public class entity_filtro
    {
        public string filtro { get; set; }
    }*/

}
