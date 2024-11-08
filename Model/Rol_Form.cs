﻿using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{

    public class entidad_guardar_rol
    {
        public int IdRol { get; set; }
        public string? Descripcion { get; set; }
        public int Estado { get; set; }
        public int IdUsuarioAccion { get; set; }
    }
    public class entidad_lst_rol_privilegio
    {

        public int IdRol { get; set; }
        [Key]
        public int IdPrivilegio { get; set; }
        public string? DesPrivilegio { get; set; }
        public string? Descripcion { get; set; }
    }

    public class entidad_guardar_rol_privilegio
    {
        public int IdRol { get; set; }
        public int IdPrivilegio { get; set; }
    }

    public class entidad_lst_modulos_accesos
    {
        [Key]
        public int IdAcceso { get; set; }
        public int IdRol { get; set; }
        public int IdModulo { get; set; }
        public string? NombreModulo { get; set; }
        public string? Icon_Modulo { get; set; }
        public string? NombreAcceso { get; set; }
        public string? Descripcion { get; set; }
        public string? Link { get; set; }
        public string? Icon_Acceso { get; set; }
    }

    public class entidad_lst_rol_modulo
    {
        public int IdModulo { get; set; }
        public string? title { get; set; }
        public string? icon { get; set; }
        public List<entidad_lst_modulo_acceso>? children { get; set; }
    }
    public class entidad_lst_modulo_acceso
    {
        public int IdAcceso { get; set; }
        public string? title { get; set; }
        public string? Descripcion { get; set; }
        public string? type { get; set; }
        public string? icon { get; set; }
        public string? link { get; set; }
    }


    public class entidad_lst_rol_acceso
    {

        public int IdRol { get; set; }
        [Key]
        public int IdAcceso { get; set; }
        public string? NombreAcceso { get; set; }
        public string? Descripcion { get; set; }

    }

    public class entidad_guardar_rol_acceso
    {
        public int IdRol { get; set; }
        public int IdAcceso { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public int IndSubMenu { get; set; }
    }
    public class entidad_eliminar_rol_acceso
    {
        public int IdRol { get; set; }
        public int IdAcceso { get; set; }
        public int IndSubMenu { get; set; }
    }

    public class entidad_lst_usuario_roles
    {
        public int IdUsuario { get; set; }
        [Key]
        public int IdRol { get; set; }
        public string? Description { get; set; }
        public int? UsuarioAccion { get; set; }
    }

    public class entidad_guardar_usuario_rol
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public int UsuarioAccion { get; set; }
    }
}
