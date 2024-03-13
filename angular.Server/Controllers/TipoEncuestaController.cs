using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json;

namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoEncuestaController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public TipoEncuestaController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost("SaveTipoEncuesta")]
        public ActionResult SaveTipoEncuesta([FromBody] JsonElement value)
        {
            try
            {
                TipoEncuesta localidadSave = JsonConvert.DeserializeObject<TipoEncuesta>(value.GetProperty("tipoEncuesta").ToString());
                context.TipoEncuesta.Add(localidadSave);
                context.SaveChanges();
                
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = localidadSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost("EditTipoEncuesta")]
        public ActionResult EditTipoEncuesta([FromBody] JsonElement value)
        {
            try
            {
                TipoEncuesta localidadSave = JsonConvert.DeserializeObject<TipoEncuesta>(value.GetProperty("tipoEncuesta").ToString());
                context.TipoEncuesta.Update(localidadSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = localidadSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost("GetTipoEncuesta")]
        public ActionResult GetTipoEncuesta([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                object ListTipoEncuesta = context.TipoEncuesta.Where(c => c.idUsuario == user_id).ToList();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListTipoEncuesta });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost("GetTipoEncuestaByState")]
        public ActionResult GetTipoEncuestaByState([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                object ListTipoEncuesta = context.TipoEncuesta.Where(c => c.idUsuario == user_id).Where(c => c.flg_estado == 1).ToList();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListTipoEncuesta });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

    }
}
