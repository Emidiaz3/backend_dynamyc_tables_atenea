using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Form> Form { get; set; }
        public DbSet<Users_Form> Users_Form { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Answers> Answers { get; set; }
        public DbSet<Form_Aparence> Form_Aparence { get; set; }
        public DbSet<entidad_usuario> entidad_usuarios { get; set; }
        public DbSet<entidad_rol_usuario> entidad_rol_usuarios { get; set; }
        public DbSet<entidad_lst_usuario> entidad_lst_usu { get; set; }
        public DbSet<Proyecto_Form> entidad_lst_proyecto { get; set; }
        public DbSet<lst_usuarios_proyectos> entidad_lst_usuarios_proyectos { get; set; }
        public DbSet<entidad_lst_rol_privilegio> entidad_lst_rol_privilegios { get; set; } 
        public DbSet<entidad_lst_rol_acceso> entidad_lst_rol_accesos { get; set; }         
        public DbSet<entidad_lst_empresa> entidad_lst_empresa { get; set; }
        public DbSet<entidad_lst_sucursal> entidad_lst_sucursal { get; set; }
        public DbSet<entidad_lst_pais> entidad_lst_pais { get; set; }
        public DbSet<entidad_lst_codigo> entidad_lst_codigo { get; set; }
        public DbSet<entidad_lst_tipodoc> entidad_lst_tipodoc { get; set; }
        public DbSet<entidad_lst_perfil> entidad_lst_perfil { get; set; }
        public DbSet<t_mae_usuario> t_mae_usuario { get; set; }
        public DbSet<T_REL_ROL_USUARIO> T_REL_ROL_USUARIO { get; set; }
        public DbSet<entidad_lst_tb_usuario> entidad_lst_tb_usuario { get; set; }
        public DbSet<entidad_lst_modulos_accesos> entidad_lst_modulos_accesos { get; set; }
        public DbSet<entidad_lst_usuario_roles> entidad_lst_usuario_roles { get; set; }
        public DbSet<entidad_lst_dep> entidad_lst_dep { get; set; }
        public DbSet<entidad_lst_prov> entidad_lst_prov { get; set; }
        public DbSet<entidad_lst_dist> entidad_lst_dist { get; set; }
        public DbSet<T_MAE_PERSONA> T_MAE_PERSONA { get; set; }
        public DbSet<Localidad> Localidad { get; set; }
        public DbSet<Organizacion> Organizacion { get; set; }
        public DbSet<Organizacion_localidad> Organizacion_Localidad { get; set; }
        public DbSet<AnswerAnioMes> AnswerAnioMes { get; set; }
        public DbSet<Usuario_Encuesta> Usuario_Encuesta { get; set; }
        public DbSet<TipoEncuesta> TipoEncuesta { get; set; }
        public DbSet<ReporteFinal> ReporteFinalL { get; set; }
        public DbSet<ReporteFinalDetail> ReporteFinal { get; set; }
        public DbSet<entity_get_proys> GetDBProys { get; set; }
        public DbSet<ColumnType> column_types { get; set; }
        public DbSet<QuestionType> question_types { get; set; }
        public DbSet<Documents> documents {  get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Persona_Organizacion> Persona_Organizacion { get; set; }


    }
}
