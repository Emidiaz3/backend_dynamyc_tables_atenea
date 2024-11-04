using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestCuestionario.Model
{
    [Table("T_MAE_USUARIO_TOKEN")]
    public class T_MAE_USUARIO_TOKEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdToken { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }  

        [Required]
        [StringLength(255)]
        public string Token { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        public bool Activo { get; set; } = true;
    }
}
