using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly AppDbContext context;
        public RolController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetListRol")]
        public async Task<ActionResult<dynamic>> GetListRol(int IdUsuarioAccion)//entidad_rol_usuario
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_rol_usuario> datos = new List<entidad_rol_usuario>();
                var rol_usuario_data = context.entidad_rol_usuarios
                                .FromSqlInterpolated($"Exec SP_ROL_SEL_01 @IdUsuario={0}")
                                .AsAsyncEnumerable();

                await foreach (var rol_usuario in rol_usuario_data)
                {
                    if (Convert.ToInt32(rol_usuario.IdUsuarioAccion) == IdUsuarioAccion)
                    {
                        datos.Add(rol_usuario);
                    }
                }
                return Ok(datos);
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }

        [HttpPost("PostGuardarRol")]
        public async Task<ActionResult<ItemResponse>> PostGuardarRol(entidad_guardar_rol ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ROL_INS_02 
                    @IdRol={ent.IdRol},
                    @Descripcion={ent.Descripcion},
                    @Estado={ent.Estado},
                    @IdUsuarioAccion={ent.IdUsuarioAccion},
                    @resp={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response);
        }

        [HttpGet("GetListRolPrivilegio")]
        public async Task<ActionResult<dynamic>> GetListRolPrivilegio(int IdRol)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_rol_privilegio> datos = new List<entidad_lst_rol_privilegio>();
                var rol_usuario_data = context.entidad_lst_rol_privilegios
                                .FromSqlInterpolated($"Exec SP_PRIVILEGIOS_SEL_02 @IdRol={IdRol}")
                                .AsAsyncEnumerable();

                await foreach (var dato in rol_usuario_data)
                {
                    datos.Add(dato);
                }
                return Ok(datos);
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }

        [HttpPost("PostGuardarRol_Privilegio")]
        public async Task<ActionResult<ItemResponse>> PostGuardarRol_Privilegio(entidad_guardar_rol_privilegio ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ROL_PRIVILEGIO_INS_02 
                    @IdRol={ent.IdRol},
                    @IdPrivilegio={ent.IdPrivilegio},
                    @resp={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response); 
        }

        [HttpPost("PostEliminarRol_Privilegio")]
        public async Task<ActionResult<ItemResponse>> PostEliminarRol_Privilegio(entidad_guardar_rol_privilegio ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ROL_PRIVILEGIO_DEL_02 
                    @IdRol={ent.IdRol},
                    @IdPrivilegio={ent.IdPrivilegio},
                    @resp={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response); 
        }

        [HttpGet("GetListRolAcceso")]
        public async Task<ActionResult<dynamic>> GetListRolAcceso(int IdRol)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_rol_acceso> datos = new List<entidad_lst_rol_acceso>();
                var rol_usuario_data = context.entidad_lst_rol_accesos
                                .FromSqlInterpolated($"Exec SP_ACCESO_SEL_01 @IdRol={IdRol}")
                                .AsAsyncEnumerable();

                await foreach (var dato in rol_usuario_data)
                {
                    datos.Add(dato);
                }
                return Ok(datos);
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response);
            }
        }

        [HttpPost("PostGuardarRol_Acceso")]
        public async Task<ActionResult<ItemResponse>> PostGuardarRol_Acceso(entidad_guardar_rol_acceso ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ROL_ACCESO_INS_03 
                    @IdRol={ent.IdRol},
                    @IdAcceso={ent.IdAcceso},
                    @IdUsuarioCreacion={ent.IdUsuarioCreacion},                    
                    @resp={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response);
        }

        [HttpPost("PostEliminarRol_Acceso")]
        public async Task<ActionResult<ItemResponse>> PostEliminarRol_Acceso(entidad_eliminar_rol_acceso ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ROL_ACCESO_DEL_03 
                    @IdRol={ent.IdRol},
                    @IdAcceso={ent.IdAcceso},
                    @resp={parametroResp} OUTPUT");
                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response);
        }


        [HttpGet("GetListUsuarioRoles")]
        public async Task<ActionResult<dynamic>> GetListUsuarioRoles(int IdUsuario)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_usuario_roles> datos = new List<entidad_lst_usuario_roles>();
                var rol_usuario_data = context.entidad_lst_usuario_roles
                                .FromSqlInterpolated($"Exec SP_ROL_USUARIO_SEL_03 @IdUsuario={IdUsuario}")
                                .AsAsyncEnumerable();

                await foreach (var dato in rol_usuario_data)
                {
                    datos.Add(dato);
                }
                return Ok(datos);
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }

        [HttpPost("PostGuardarUsuario_Rol")]
        public async Task<ActionResult<ItemResponse>> PostGuardarUsuario_Rol(entidad_guardar_usuario_rol ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_ROL_INS_01 
                    @IdUsuario={ent.IdUsuario},
                    @IdRol={ent.IdRol},
                    @UsuarioAccion={ent.UsuarioAccion},                    
                    @resp={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response); 
        }

        [HttpPost("PostEliminarUsuario_Rol")]
        public async Task<ActionResult<ItemResponse>> PostEliminarUsuario_Rol(entidad_guardar_usuario_rol ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_USUARIO_ROL_DEL_01 
                    @IdUsuario={ent.IdUsuario},
                    @IdRol={ent.IdRol},
                    @UsuarioAccion={ent.UsuarioAccion},                    
                    @resp={parametroResp} OUTPUT");

                if (parametroResp.Value != DBNull.Value)
                {
                    response.status = (int)parametroResp.Value;
                }
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
            }

            return Ok(response); 
        }

       
    }
}
