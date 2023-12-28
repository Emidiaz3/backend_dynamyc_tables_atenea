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
    public class TipoEncuestaController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public TipoEncuestaController(AppDbContext context)
        {
            this.context = context;
        }
        [Route("SaveTipoEncuesta")]
        [HttpPost]
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
        [Route("EditTipoEncuesta")]
        [HttpPost]
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

        [HttpPost]
        [Route("GetTipoEncuesta")]
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

        [HttpPost]
        [Route("GetTipoEncuestaByState")]
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
