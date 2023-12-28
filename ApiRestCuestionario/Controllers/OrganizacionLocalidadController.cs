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
    public class OrganizacionLocalidadController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public OrganizacionLocalidadController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpPost]
        [Route("GetOrganizacionLocalidad")]
        public ActionResult GetOrganizacionLocalidad([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                object ListPerson = context.T_MAE_PERSONA.Where(c => c.idusuario == user_id).ToList();
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListPerson });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
    }
}
