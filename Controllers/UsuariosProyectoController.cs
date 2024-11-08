﻿using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;


namespace ApiRestCuestionario.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosProyectoController : ControllerBase
    {
        private readonly AppDbContext context;
        public UsuariosProyectoController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetListUsuariosProyectos")]
        public async Task<ActionResult<dynamic>> GetListUsuariosProyectos(int userId)
        {
            var response = new ItemResponse();

            try
            {
                List<lst_usuarios_proyectos> data = await context.entidad_lst_usuarios_proyectos
                .FromSqlInterpolated($"Exec SP_PROYECTO_USUARIO_SEL_01 @userId={userId}")
                .ToListAsync();

                response.status = 1;
                response.data = data;
            
                return Ok(response);
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

        [HttpPost("PostGuardarUsuarioProyecto")]
        public async Task<ActionResult<ItemResponse>> PostGuardarUsuarioProyecto(entidad_guardar_proyecto_usuario ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_GUARDAR_PROYECTO_USUARIO 
                    @IdRelUsuProy={ent.IdRelUsuProy},
                    @IdProyecto={ent.IdProyecto},
                    @IdUsuario={ent.IdUsuario},
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

        [HttpPost("PostEliminarUsuarioProyecto")]
        public async Task<ActionResult<ItemResponse>> PostEliminarUsuarioProyecto(entity_delete_proy_usu ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ELIMINAR_PROYECTO_USUARIO 
                    @IdRelUsuProy={ent.IdRelUsuProy},
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
