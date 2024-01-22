using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext context;
        public EmpresaController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetListEmpresa")]
        public async Task<ActionResult<dynamic>> GetListEmpresa(int IdUsuarioAccion)
        {
            var response = new ItemResponse();
            try
            {
                string filtro = $"and IdUsuarioModificacion={IdUsuarioAccion}";
                List<entidad_lst_empresa> datos = new List<entidad_lst_empresa>();
                var empresa_data = context.entidad_lst_empresa
                .FromSqlInterpolated($"Exec SP_EMPRESA_SEL_01 @FILTRO={filtro}")
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
                return Ok(response); 
            }
        }

        [HttpPost("PostGuardarEmpresa")]
        public async Task<ActionResult<ItemResponse>> PostGuardarEmpresa(entidad_guardar_empresa ent)
        {
            var response = new ItemResponse();
            response.status = 0;
            try
            {
                var parametroResp = new SqlParameter("@resp", SqlDbType.Int);
                parametroResp.Direction = ParameterDirection.Output;

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
