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
    public class ProyectoController : ControllerBase
    {
        private readonly AppDbContext context;
        public ProyectoController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetListProyectos")]
        public async Task<ActionResult<dynamic>> GetListProyectos(int IdUsuarioAccion)
        {
            var response = new ItemResponse();

            try
            {
                string filtro = " and T_MAE_PROYECTO.UsuarioAccion=" + IdUsuarioAccion;
                List<Proyecto_Form> datos = new List<Proyecto_Form>();
                var proyecto_data = context.entidad_lst_proyecto
                .FromSqlInterpolated($"Exec SP_PROYECTO_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var proyecto in proyecto_data)
                {
                    datos.Add(proyecto);
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

        [HttpPost("PostGuardarProyecto")]
        public async Task<ActionResult<ItemResponse>> PostGuardarProyecto(entidad_guardar_proyecto ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database.ExecuteSqlInterpolatedAsync($@"Exec SP_GUARDAR_PROYECTO @IdProyecto={ent.IdProyecto}, @NombreProyecto={ent.NombreProyecto}, @NombreDB={ent.NombreDB}, @IdEmpresa={ent.IdEmpresa}, @EstadoProyecto={ent.EstadoProyecto}, @UsuarioAccion={ent.UsuarioAccion}, @resp={parametroResp} OUTPUT");

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

        [HttpPost("PostEliminarProyecto")]
        public async Task<ActionResult<ItemResponse>> PostEliminarProyecto(entity_delete_proy ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_ELIMINAR_PROYECTO 
                    @IdProyecto={ent.IdProyecto},
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

        [HttpGet("GetListDBProyectos")]
        public async Task<ActionResult<dynamic>> GetListDBProyectos()
        {
            var response = new ItemResponse();

            try
            {
                var proyecto_data = await context.GetDBProys.FromSqlInterpolated($"Exec SP_LISTAR_PROYECTOS").ToListAsync();

                return Ok(proyecto_data);
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
    }
}
