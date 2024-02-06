using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Users_Form
    {
        public Users_Form(int users_id, int form_id)
        {
            this.users_id = users_id;
            this.form_id = form_id;
        }
        public Users_Form(int users_id, int form_id,string state)
        {
            this.users_id = users_id;
            this.form_id = form_id;
            this.state = state;
        }
        public Users_Form(int id,int users_id, int form_id, string state)
        {
            this.id = id;
            this.users_id = users_id;
            this.form_id = form_id;
            this.state = state;
        }
        Users_Form() { }
        public int id { get; set; }
        public int users_id { get; set; }
        public int form_id { get; set; }
        public string state { get; set; }

    }

    public class entidad_usuario
    {
        [Key]
        public int IdUser { get; set; }
        public string Username { get; set; }
        public int Country { get; set; }
        public int IdIdioma { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdSucursal { get; set; }
    }
    public class entidad_lst_usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdPais { get; set; }
        public string? Pais { get; set; }
        public int IdEmpresa { get; set; }
        public string? DescripcionEmpresa { get; set; }
        public int? IdSucursal { get; set; }
        public string? DescripcionSucursal { get; set; }
        public string? NombreUsuario { get; set; }
        //public string PassUsuario { get; set; }
        public int EstadoUsuario { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public int IdTipoDocIdentidad { get; set; }
        public string? DesTipoDocIdentidad { get; set; }
        public string? NumDocIdentidad { get; set; }
        public string? Telefono { get; set; }
        public int? UsuarioCreacion { get; set; }
        public int? UsuarioAccion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAccion { get; set; }
        public string? CodigoCambioPassword { get; set; }
        public int? FlgCambioPassword { get; set; }
        public DateTime? FechaCambioPassword { get; set; }
        public string? IdRol { get; set; }
        public string? DesRol { get; set; }
        public string? token { get; set; }
    }

    public class entidad_lst_usuario_acceso
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdPais { get; set; }
        public string? Pais { get; set; }
        public int IdEmpresa { get; set; }
        public string? DescripcionEmpresa { get; set; }
        public int? IdSucursal { get; set; }
        public string? DescripcionSucursal { get; set; }
        public string? NombreUsuario { get; set; }
        //public string PassUsuario { get; set; }
        public int EstadoUsuario { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public int IdTipoDocIdentidad { get; set; }
        public string? DesTipoDocIdentidad { get; set; }
        public string? NumDocIdentidad { get; set; }
        public string? Telefono { get; set; }
        public int? UsuarioCreacion { get; set; }
        public int? UsuarioAccion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAccion { get; set; }
        public string? CodigoCambioPassword { get; set; }
        public int? FlgCambioPassword { get; set; }
        public DateTime? FechaCambioPassword { get; set; }
        public string? IdRol { get; set; }
        public string? DesRol { get; set; }
        public string? token { get; set; }
        public List<entidad_lst_rol_modulo>? datos_modulo { get; set; }
    }

    public class entidad_lst_tb_usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdPais { get; set; }
        public string? Pais { get; set; }
        public int IdEmpresa { get; set; }
        public string? DescripcionEmpresa { get; set; }
        public int? IdSucursal { get; set; }
        public string? DescripcionSucursal { get; set; }
        public string? NombreUsuario { get; set; }
        public string? PassUsuario { get; set; }
        public int EstadoUsuario { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Nombre { get; set; }        
        public string? Nombres_Completos { get; set; }
        public string? Email { get; set; }
        public int IdTipoDocIdentidad { get; set; }
        public string? DesTipoDocIdentidad { get; set; }
        public string? NumDocIdentidad { get; set; }
        public string? Telefono { get; set; }
        public int? UsuarioCreacion { get; set; }
        public int? UsuarioAccion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAccion { get; set; }
        public string? CodigoCambioPassword { get; set; }
        public int? FlgCambioPassword { get; set; }
        public DateTime? FechaCambioPassword { get; set; }
        public int? IdRol { get; set; }
        public string? DesRol { get; set; }

    }

    public class T_REL_ROL_USUARIO
    {
       
        public int IdRol { get; set; }
        [Key]
        public int IdUsuario { get; set; }
        
        public int estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaAccion { get; set; }
        public DateTime UsuarioCreacion { get; set; }

        public int? UsuarioAccion { get; set; }
    }
    public class entidad_rol_usuario
    {

        public int IdUsuario { get; set; }
        [Key]
        public int IdRol { get; set; }
        public string? Description { get; set; }
        public int? IdUsuarioAccion { get; set; }
    }
    public class entidad_guardar_usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public int IdPais { get; set; }
        public int IdEmpresa { get; set; }
        public int IdSucursal { get; set; }
        public string? NombreUsuario { get; set; }
        public string? PassUsuario { get; set; }
        public int EstadoUsuario { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public int IdTipoDocIdentidad { get; set; }
        public string? NumDocIdentidad { get; set; }
        public string? Telefono { get; set; }
        public int UsuarioAccion { get; set; }
        public int IdRol { get; set; }
    }
    public class t_mae_usuario
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
    public class Login_User
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "La clave es obligatorio.")]
        public string? PassUsuario { get; set; }
    }

    public class entity_cod_pass
    {
        public int IdUsuario { get; set; }
        public string? CodigoCambioPassword { get; set; }
    }

    public class entity_update_pass
    {
        public int IdUsuario { get; set; }
        public string? PassUsuarioOld { get; set; }
        public string? PassUsuarioNew { get; set; }
    }
    public class entidad_lst_perfil
    {
        [Key]
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? FotoPerfil { get; set; }

    }

    public class entidad_actualizar_perfil
    {
        [Key]
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public int? UsuarioAccion { get; set; }
    }
    public class entidad_correo
    {
        public string? Email { get; set; }
    }

    public class entidad_dato_id_user
    {
        public int IdUsuario { get; set; }
    }
    public class entidad_lst_codigo
    {
        [Key]
        //public int? IdUsuario { get; set; }
        public string? CodigoCambioPassword { get; set; }
        public int? FlgCambioPassword { get; set; }
        public DateTime? FechaCambioPassword { get; set; }
    }

    public class entity_password
    {
        public int IdUsuario { get; set; }
        public string? PassUsuario { get; set; }
    }

}
