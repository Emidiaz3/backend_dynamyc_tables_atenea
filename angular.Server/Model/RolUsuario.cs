using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestCuestionario.Model
{
    [Table("T_REL_ROL_USUARIO")]
    public class RolUsuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaAccion { get; set; }
        public DateTime UsuarioCreacion { get; set; }
        public int? UsuarioAccion { get; set; }
    }
}
