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
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext context;
        public EmpresaController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("GetListEmpresa")]
        public async Task<ActionResult<dynamic>> GetListEmpresa(int IdUsuarioAccion)
        {
            var response = new ItemResponse();

            try
            {
                string filtro = "and IdUsuarioModificacion=" + IdUsuarioAccion;

                List<entidad_lst_empresa> datos = new List<entidad_lst_empresa>();
                var empresa_data = context.entidad_lst_empresa
                .FromSqlInterpolated($"Exec SP_EMPRESA_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in empresa_data)
                {
                    datos.Add(dato);
                }

                //if (datos.Count() > 0)
                //{
                    return Ok(datos);
                //}
                /*else
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
                return Ok(response); 
            }
        }

        [HttpPost]
        [Route("PostGuardarEmpresa")]
        public async Task<ActionResult<ItemResponse>> PostGuardarEmpresa(entidad_guardar_empresa ent)
        {
            //var val_resp = "";
            //int respuesta = 0;
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                /*var IdPais = new SqlParameter("@IdPais", ent.IdPais);
                var IdTipodocumentoEmpresa = new SqlParameter("@IdTipodocumentoEmpresa", ent.IdTipodocumentoEmpresa);
                var NroDocumento = new SqlParameter("@NroDocumento", ent.NroDocumento);
                var DescripcionEmpresa = new SqlParameter("@DescripcionEmpresa", ent.DescripcionEmpresa);
                var FlgEstado = new SqlParameter("@FlgEstado", ent.FlgEstado);
                var IdUsuarioAccion = new SqlParameter("@IdUsuarioAccion", ent.IdUsuarioAccion);
                var Direccion = new SqlParameter("@Direccion", ent.Direccion);

                var parametroResp = new SqlParameter("@IdEmpresa", SqlDbType.Int) { Direction = ParameterDirection.Output };               

                context.Database
                    .ExecuteSqlRaw("exec SP_GUARDAR_EMPRESA @IdPais={0}, @IdTipodocumentoEmpresa={1},@NroDocumento={2},@DescripcionEmpresa={3},@FlgEstado={4},@IdUsuarioAccion={5},@Direccion{6}, @IdEmpresa={7} out", IdPais, IdTipodocumentoEmpresa, NroDocumento, DescripcionEmpresa, FlgEstado, IdUsuarioAccion, Direccion, parametroResp);

                if (parametroResp.Value != DBNull.Value)
                {                    
                    respuesta = (int)parametroResp.Value;
                }*/

                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;
                //parametroResp.Value = ent.IdEmpresa;

                await context.Database
                    .ExecuteSqlInterpolatedAsync($@"Exec SP_GUARDAR_EMPRESA_02 
                    @IdEmpresa={ent.IdEmpresa},
                    @IdPais={ent.IdPais},
                    @IdTipodocumentoEmpresa={ent.IdTipodocumentoEmpresa},
                    @NroDocumento={ent.NroDocumento},
                    @DescripcionEmpresa={ent.DescripcionEmpresa},
                    @FlgEstado={ent.FlgEstado},
                    @IdUsuarioAccion={ent.IdUsuarioAccion},
                    @Direccion={ent.Direccion},
                    @resp={parametroResp} OUTPUT");//  ent.IdEmpresa parametroResp

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
                //respuesta = 0;
                //throw;
            }

            return Ok(response); //val_resp;
            //return Ok(respuesta);
        }


        // GET: api/<EmpresaController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EmpresaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EmpresaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmpresaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmpresaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
