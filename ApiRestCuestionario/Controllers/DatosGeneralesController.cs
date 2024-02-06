using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatosGeneralesController : ControllerBase
    {
        private readonly AppDbContext context;
        public DatosGeneralesController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("Countries")]
        public async Task<ActionResult<dynamic>> GetCountries()
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_pais> countries = await context.entidad_lst_pais
                .FromSqlInterpolated($"Exec SP_PAIS_SEL_01")
                .ToListAsync();

                response.status = 1;
                response.data = countries;
               

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

        [HttpGet("documentTypes")]
        public async Task<ActionResult<dynamic>> GetDocumenTypes()
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_tipodoc> documentTypes = await context.entidad_lst_tipodoc
                .FromSqlInterpolated($"Exec SP_TIPODOCUMENTO_SEL_01")
                .ToListAsync();

                response.status = 1;
                response.data = documentTypes;
             
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

        [HttpGet("GetListDepartamento")]
        public async Task<ActionResult<dynamic>> GetListDepartamento(int IdPais)
        {
            var response = new ItemResponse();

            try
            {
                string filtro = "";

                if(IdPais> 0)
                {
                    filtro = " and IdPais="+ IdPais;
                }

                List<entidad_lst_dep> datos = new List<entidad_lst_dep>();
                var data = context.entidad_lst_dep
                .FromSqlInterpolated($"Exec SP_DEPARTAMENTO_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data)
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

        [HttpGet("GetListProvincia")]
        public async Task<ActionResult<dynamic>> GetListProvincia(int IdDep)
        {
            var response = new ItemResponse();

            try
            {
                string filtro = "";

                if (IdDep > 0)
                {
                    filtro = $" and IdDep={IdDep}";
                }

                List<entidad_lst_prov> datos = new List<entidad_lst_prov>();
                var data = context.entidad_lst_prov
                .FromSqlInterpolated($"Exec SP_PROVINCIA_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data)
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

        [HttpGet("GetListDistrito")]
        public async Task<ActionResult<dynamic>> GetListDistrito(int IdProv)
        {
            var response = new ItemResponse();

            try
            {
                string filtro = "";

                if (IdProv > 0)
                {
                    filtro = " and IdProv=" + IdProv;
                }

                List<entidad_lst_dist> datos = new List<entidad_lst_dist>();
                var data = context.entidad_lst_dist
                .FromSqlInterpolated($"Exec SP_DISTRITO_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data)
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

        [HttpGet("GetDepartamentos")]
        public async Task<ActionResult> GetDepartamentos()
        {
            try
            {
                var list = await context.entidad_lst_dep.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_DEPARTAMENTOS]").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos correctamente ...!", data = list });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = "Error ...!", data = e.ToString() });
            }
        }

        [HttpGet("GetProvinciasByDepartamento")]
        public async Task<ActionResult> GetProvinciasByDepartamento(int idDep)
        {
            try
            {
                var list = await context.entidad_lst_prov.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_PROVINCIAS_BY_DEP] @IdDep={idDep}").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos correctamente ...!", data = list });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = "Error ...!", data = e.ToString() });
            }
        }

        [HttpGet("GetDistritosByProvincia")]
        public async Task<ActionResult> GetDistritosByProvincia(int idProv)
        {
            try
            {
                var list = await context.entidad_lst_dist.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_DISTRITOS_BY_PROV] @IdProv={idProv}").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos correctamente ...!", data = list });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = "Error ...!", data = e.ToString() });
            }
        }
    }
}
