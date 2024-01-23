using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public PersonaController(AppDbContext context)
        {
            this.context = context;
        }
        // GET: api/<PersonaController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PersonaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PersonaController>
        [Route("SavePerson")]
        [HttpPost]
        public async Task<ActionResult> SavePerson([FromBody] JsonElement value)
        {
            try
            {
                Persona persona = JsonConvert.DeserializeObject<Persona>(value.GetProperty("person").ToString());

                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = persona.Id ?? (object)DBNull.Value
                };

                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_PERSONA] 
                    @Id={idParameter} OUTPUT, 
                    @Id_Persona={persona.Id_Persona},
                    @NomPersona={persona.NomPersona},
                    @Proyecto={persona.Proyecto},
                    @Localidad={persona.Localidad},
                    @Nivel_Riesgo_General={persona.Nivel_Riesgo_General},
                    @Latitud={persona.Latitud},
                    @Longitud={persona.Longitud},
                    @TipoDocumento={persona.TipoDocumento},
                    @NumeroDocumento={persona.NumeroDocumento},
                    @Estado={persona.Estado},
                    @IdUsuario={persona.IdUsuario}
                ;");

                if (persona.Id != null)
                {
                    List<Persona_Organizacion> removeList = JsonConvert.DeserializeObject<List<Persona_Organizacion>>(value.GetProperty("deletePersonaOrganizacionList").ToString());
                    foreach (Persona_Organizacion persona_Organizacion in removeList)
                    {
                        await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_DELETE_SPECIFIC_PERSONA_ORGANIZACION] 
                        @IdPersona={idParameter},
                        @IdOrganizacion={persona_Organizacion.IdOrganizacion},
                        @Proyecto={persona.Proyecto};");
                    }
                }

                List<Persona_Organizacion> insertList = JsonConvert.DeserializeObject<List<Persona_Organizacion>>(value.GetProperty("insertPersonaOrganizacionList").ToString());

                foreach (Persona_Organizacion persona_Organizacion in insertList)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_PERSONA_ORGANIZACION] 
                    @IdPersona={idParameter},
                    @IdOrganizacion={persona_Organizacion.IdOrganizacion},
                    @Proyecto={persona.Proyecto};");
                }

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }

        }
        //[Route("EditPerson")]
        //[HttpPost]
        //public ActionResult EditPerson([FromBody] JsonElement value)
        //{
        //    try
        //    {
        //        int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
        //        T_MAE_PERSONA personaSave = JsonConvert.DeserializeObject<T_MAE_PERSONA>(value.GetProperty("person").ToString());
        //        context.T_MAE_PERSONA.Update(personaSave);
        //        context.SaveChanges();
        //        return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = personaSave });
        //    }
        //    catch (InvalidCastException e)
        //    {
        //        return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
        //    }

        //}
        
        [HttpGet]
        [Route("GetPerson")]
        public async Task<ActionResult> GetPerson(int IdProyecto)
        {
            try
            {
                //int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                //object ListPerson=context.T_MAE_PERSONA.ToList();
                var ListPersona = await context.Persona.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_PERSONA_POR_PROYECTO] @IdProyecto={IdProyecto}").ToListAsync();

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListPersona });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost]
        [Route("GetPersonByTipoEncuesta")]
        public ActionResult GetPersonByTipoEncuesta([FromBody] JsonElement value)
        {

            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                string[] tipoEncuesta = JsonConvert.DeserializeObject<string[]>(value.GetProperty("TipoEncuesta").ToString());
                
                List<T_MAE_PERSONA> ListPerson = context.T_MAE_PERSONA.ToList();
                List<T_MAE_PERSONA> newListPerson = new List<T_MAE_PERSONA> { };

                foreach (T_MAE_PERSONA c in ListPerson)
                {
                    if (JsonConvert.DeserializeObject<string[]>(c.TipoEncuesta).Intersect(tipoEncuesta).Any())
                    {
                        newListPerson.Add(c);

                    }
                }

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = newListPerson });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        // PUT api/<PersonaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PersonaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
