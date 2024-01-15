using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
        public ActionResult SavePerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                T_MAE_PERSONA personaSave = JsonConvert.DeserializeObject<T_MAE_PERSONA>(value.GetProperty("person").ToString());
                personaSave.fecharegistro = DateTime.Now;
                context.T_MAE_PERSONA.Add(personaSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = personaSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }
            
        }
        [Route("EditPerson")]
        [HttpPost]
        public ActionResult EditPerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                T_MAE_PERSONA personaSave = JsonConvert.DeserializeObject<T_MAE_PERSONA>(value.GetProperty("person").ToString());
                context.T_MAE_PERSONA.Update(personaSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = personaSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        
        [HttpPost]
        [Route("GetPerson")]
        public ActionResult GetPerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                object ListPerson=context.T_MAE_PERSONA.ToList();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListPerson });
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
