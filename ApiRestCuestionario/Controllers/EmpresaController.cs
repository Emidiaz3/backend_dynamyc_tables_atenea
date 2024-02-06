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

        [HttpGet("companies")]
        public async Task<ActionResult<dynamic>> GetListEmpresa(int userId)
        {
            var response = new ItemResponse();
            try
            {
                List<entidad_lst_empresa> empresa_data = await context.entidad_lst_empresa
                .FromSqlInterpolated($"Exec SP_EMPRESA_SEL_01 @userId={userId}")
                .ToListAsync();

                response.status = 1;
                response.data = empresa_data;
            
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
