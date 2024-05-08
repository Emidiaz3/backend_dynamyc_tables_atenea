using ApiRestCuestionario.Context;
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
 

    public class SucursalController : ControllerBase
    {
        private readonly AppDbContext context;
        public SucursalController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetListSucursal")]
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


        [HttpPost("PostGuardarSucursal")]
        public async Task<ActionResult<ItemResponse>> PostGuardarSucursal(entidad_guardar_sucursal ent)
        {
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
