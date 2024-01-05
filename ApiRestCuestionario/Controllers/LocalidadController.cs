using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalidadController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public LocalidadController(AppDbContext context)
        {
            this.context = context;
        }
        // POST api/<PersonaController>
        [Route("SaveLocalidad")]
        [HttpPost]
        public ActionResult SavePerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                Localidad localidadSave = JsonConvert.DeserializeObject<Localidad>(value.GetProperty("localidad").ToString());
                localidadSave.fecharegistro = DateTime.Now;

                Localidad localidadValidate = null;
                localidadValidate = context.Localidad.ToList().Where(c => c.ID_LOCALIDAD == localidadSave.ID_LOCALIDAD).FirstOrDefault();

                if (localidadValidate != null)
                {
                    return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });
                }


                context.Localidad.Add(localidadSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = localidadSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [Route("EditLocalidad")]
        [HttpPost]
        public ActionResult EditPerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                Localidad localidadSave = JsonConvert.DeserializeObject<Localidad>(value.GetProperty("localidad").ToString());

                Localidad localidadValidate = null;
                localidadValidate = context.Localidad.ToList().AsReadOnly().Where(c => c.ID_LOCALIDAD == localidadSave.ID_LOCALIDAD).FirstOrDefault();
                if (localidadValidate != null)
                {
                    if (localidadSave.idlocalidad != localidadValidate.idlocalidad)
                    {
                        return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });

                    }
                }

                context.Localidad.Update(localidadSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = localidadSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost]
        [Route("GetLocalidad")]
        public ActionResult GetPerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                object ListLocalidad = context.Localidad.ToList();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListLocalidad });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
    }
}
