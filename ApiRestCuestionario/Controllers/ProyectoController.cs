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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProyectoController : ControllerBase
    {
        private readonly AppDbContext context;
        //private readonly IConfiguration config;
        public ProyectoController(AppDbContext context) //, IConfiguration _config
        {
            this.context = context;
            //this.config = _config;
        }

        [HttpGet]
        [Route("GetListProyectos")]
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
                /*if (datos.Count() > 0)
                {
                    return Ok(datos);
                }
                else
                {
                    response.status = 0;
                    response.message = "Sin Registros";
                    return Ok(response);
                }*/
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

        [HttpPost]
        [Route("PostGuardarProyecto")]
        public async Task<ActionResult<ItemResponse>> PostGuardarProyecto(entidad_guardar_proyecto ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                     .ExecuteSqlInterpolatedAsync($@"Exec SP_GUARDAR_PROYECTO 
                    @IdProyecto={ent.IdProyecto},
                    @NombreProyecto={ent.NombreProyecto},
                    @NombreDB={ent.NombreBaseDatos},
                    @IdEmpresa={ent.IdEmpresa},
                    @EstadoProyecto={ent.EstadoProyecto},
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

            return Ok(response); //val_resp;
            //return Ok(respuesta);
        }

        [HttpPost]
        [Route("PostEliminarProyecto")]
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

            return Ok(response); //val_resp;
            //return Ok(respuesta);
        }

        [HttpGet]
        [Route("GetListDBProyectos")]
        public async Task<ActionResult<dynamic>> GetListDBProyectos()
        {
            var response = new ItemResponse();

            try
            {
                var proyecto_data = await context.GetDBProys
                .FromSqlInterpolated($"Exec SP_LISTAR_PROYECTOS")
                .ToListAsync();

                return Ok(proyecto_data);
                /*if (datos.Count() > 0)
                {
                    return Ok(datos);
                }
                else
                {
                    response.status = 0;
                    response.message = "Sin Registros";
                    return Ok(response);
                }*/
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

        //***********************************************
        // GET: api/<ProyectoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProyectoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProyectoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProyectoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProyectoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
