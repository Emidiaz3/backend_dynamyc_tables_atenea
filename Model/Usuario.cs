using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestCuestionario.Model
{
    [Table("T_MAE_USUARIO")]
    public class Usuario
    {
   
            [Key]
            public int IdUsuario { get; set; }
            public int IdPais { get; set; }
            public int IdEmpresa { get; set; }
            public int? IdSucursal { get; set; }
            public string? NombreUsuario { get; set; }
            public string? PassUsuario { get; set; }
            public bool? EstadoUsuario { get; set; }
            public string? ApellidoPaterno { get; set; }
            public string? ApellidoMaterno { get; set; }
            public string? Nombre { get; set; }
            public string? Email { get; set; }
            public int IdTipoDocIdentidad { get; set; }
            public string? NumDocIdentidad { get; set; }
            public string? Telefono { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public int? UsuarioCreacion { get; set; }
            public DateTime FechaAccion { get; set; }
            public int UsuarioAccion { get; set; }
            public string? CodigoCambioPassword { get; set; }
            public bool? FlgCambioPassword { get; set; }
            public DateTime? FechaCambioPassword { get; set; }
            public string? FotoPerfil { get; set; }

    }
}
