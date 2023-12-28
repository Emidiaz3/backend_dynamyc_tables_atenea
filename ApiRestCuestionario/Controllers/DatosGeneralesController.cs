using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosGeneralesController : ControllerBase
    {

        private readonly AppDbContext context;
        //private readonly IConfiguration config;
        public DatosGeneralesController(AppDbContext context) //, IConfiguration _config
        {
            this.context = context;
            //this.config = _config;
        }

        [HttpGet]
        [Route("GetListPais")]
        public async Task<ActionResult<dynamic>> GetListPais(string filtro)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_pais> datos = new List<entidad_lst_pais>();
                var data_pais = context.entidad_lst_pais
                .FromSqlInterpolated($"Exec SP_PAIS_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data_pais)
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

        [HttpGet]
        [Route("GetListTipoDocumento")]
        public async Task<ActionResult<dynamic>> GetListTipoDocumento(string filtro)
        {
            var response = new ItemResponse();

            try
            {
                List<entidad_lst_tipodoc> datos = new List<entidad_lst_tipodoc>();
                var data_pais = context.entidad_lst_tipodoc
                .FromSqlInterpolated($"Exec SP_TIPODOCUMENTO_SEL_01 @FILTRO={filtro}")
                .AsAsyncEnumerable();

                response.status = 1;

                await foreach (var dato in data_pais)
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

        [HttpGet]
        [Route("GetListDepartamento")]
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

        [HttpGet]
        [Route("GetListProvincia")]
        public async Task<ActionResult<dynamic>> GetListProvincia(int IdDep)
        {
            var response = new ItemResponse();

            try
            {
                string filtro = "";

                if (IdDep > 0)
                {
                    filtro = " and IdDep=" + IdDep;
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

        [HttpGet]
        [Route("GetListDistrito")]
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

        //**********************************************************************

        // GET: api/<DatosGeneralesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DatosGeneralesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DatosGeneralesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DatosGeneralesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DatosGeneralesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
