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
    public class SucursalController : ControllerBase
    {
        private readonly AppDbContext context;
        public SucursalController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("GetListSucursal")]
        public async Task<ActionResult<dynamic>> GetListSucursal(string filtro)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_sucursal> datos = new List<entidad_lst_sucursal>();
                var empresa_data = context.entidad_lst_sucursal
                .FromSqlInterpolated($"Exec SP_SUCURSAL_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in empresa_data)
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


        [HttpPost]
        [Route("PostGuardarSucursal")]
        public async Task<ActionResult<ItemResponse>> PostGuardarSucursal(entidad_guardar_sucursal ent)
        {
            //var val_resp = "";
            //int respuesta = 0;
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_GUARDAR_SUCURSAL_02
                        @IdSucursal={ent.IdSucursal},
                        @IdEmpresa={ent.IdEmpresa},
                        @IdPais={ent.IdPais},
                        @DescripcionSucursal={ent.DescripcionSucursal},
                        @Direccion={ent.@Direccion},
                        @IdUsuarioCreacion={ent.IdUsuarioCreacion},
                        @IdUsuarioAccion={ent.IdUsuarioAccion},
                        @FlgEstado={ent.FlgEstado},                                                
                        @resp={parametroResp} OUTPUT");//parametroResp

                //respuesta = (int)parametroResp.Value;
                if (parametroResp.Value != DBNull.Value)
                {
                    //respuesta = (int)parametroResp.Value;
                    response.status = (int)parametroResp.Value;
                }

                /*if (respuesta > 0)
                {
                    val_resp = "Guardado";
                }
                else
                {
                    val_resp = "No Guardo";
                }*/
                //return Ok(respuesta);
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
                //val_resp = errorMessages.ToString();
                //throw;
            }

            return Ok(response);//val_resp;
        }


        // GET: api/<SucursalController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SucursalController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SucursalController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SucursalController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SucursalController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
