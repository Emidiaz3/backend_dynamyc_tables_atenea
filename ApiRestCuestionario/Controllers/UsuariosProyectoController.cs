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
    public class UsuariosProyectoController : ControllerBase
    {
        private readonly AppDbContext context;
        //private readonly IConfiguration config;
        public UsuariosProyectoController(AppDbContext context) //, IConfiguration _config
        {
            this.context = context;
            //this.config = _config;
        }

        [HttpGet]
        [Route("GetListUsuariosProyectos")]
        public async Task<ActionResult<dynamic>> GetListUsuariosProyectos(string filtro)
        {
            var response = new ItemResponse();

            try
            {
                List<lst_usuarios_proyectos> datos = new List<lst_usuarios_proyectos>();
                var proyecto_data = context.entidad_lst_usuarios_proyectos
                .FromSqlInterpolated($"Exec SP_PROYECTO_USUARIO_SEL_01 @FILTRO={filtro}")
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

        [HttpPost]
        [Route("PostGuardarUsuarioProyecto")]
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

            return Ok(response); //val_resp;
            //return Ok(respuesta);
        }

        [HttpPost]
        [Route("PostEliminarUsuarioProyecto")]
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

            return Ok(response); //val_resp;
            //return Ok(respuesta);
        }

        //***********************************************
        // GET: api/<UsuariosProyectoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsuariosProyectoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsuariosProyectoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsuariosProyectoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsuariosProyectoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
