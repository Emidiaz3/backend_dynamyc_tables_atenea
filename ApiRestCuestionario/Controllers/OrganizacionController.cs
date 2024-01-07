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
    public class OrganizacionController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public OrganizacionController(AppDbContext context)
        {
            this.context = context;
        }
        // POST api/<PersonaController>
        [Route("SaveOrganizacion")]
        [HttpPost]
        public ActionResult SavePerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                Organizacion organizacionSave = JsonConvert.DeserializeObject<Organizacion>(value.GetProperty("organizacion").ToString());
                List<Organizacion_localidad> organizacion_localidadSave = JsonConvert.DeserializeObject<List<Organizacion_localidad>>(value.GetProperty("organizacionLocalidad").ToString());

                organizacionSave.fecharegistro = DateTime.Now;


                Organizacion organizacionValidate = null;
                organizacionValidate= context.Organizacion.ToList().Where(c => c.ID_ORGANIZACION == organizacionSave.ID_ORGANIZACION).FirstOrDefault();

                if (organizacionValidate != null)
                {
                    return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });
                }

                context.Organizacion.Add(organizacionSave);
                context.SaveChanges();
                
                foreach (Organizacion_localidad c in organizacion_localidadSave)
                {
                    c.idOrganizacion = organizacionSave.idorganizacion;
                }
                context.Organizacion_Localidad.AddRange(organizacion_localidadSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = organizacionSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = e.ToString(), data = e.ToString() });
            }

        }
        [Route("EditOrganizacion")]
        [HttpPost]
        public ActionResult EditPerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                Organizacion organizacionSave = JsonConvert.DeserializeObject<Organizacion>(value.GetProperty("organizacion").ToString());



                Organizacion organizacionValidate = null;
                organizacionValidate = context.Organizacion.ToList().AsReadOnly().Where(c => c.ID_ORGANIZACION == organizacionSave.ID_ORGANIZACION).FirstOrDefault();
                if(organizacionValidate != null)
                {
                    if (organizacionSave.idorganizacion != organizacionValidate.idorganizacion)
                    {
                        return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });

                    }
                }
                List<Organizacion_localidad> organizacion_localidadupdate = JsonConvert.DeserializeObject<List<Organizacion_localidad>>(value.GetProperty("organizacionLocalidad").ToString());
                List < Organizacion_localidad > organizacion_localidadSave = new List<Organizacion_localidad>();
                foreach (Organizacion_localidad c in organizacion_localidadupdate)
                {
                    if(c.idOrganizacionLocalidad == 0)
                    {
                        context.Organizacion_Localidad.Add(c);
                    }
                    else
                    {
                        context.Organizacion_Localidad.Update(c);
                    }
                }
                
                context.Organizacion.Update(organizacionSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = organizacionSave });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost]
        [Route("GetOrganizacion")]
        public ActionResult GetPerson([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                object ListOrganizacion = context.Organizacion.ToList();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListOrganizacion });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [HttpPost]
        [Route("GetOrganizacionlocalidadById")]
        public ActionResult GetOrganizaciónById([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                int organizacion_id = JsonConvert.DeserializeObject<int>(value.GetProperty("organizacion").GetProperty("organizacion_id").ToString());
                
                object listLocalidad = context.Organizacion_Localidad.Where(c => organizacion_id == c.idOrganizacion).ToList();
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = listLocalidad });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [HttpPost]
        [Route("DeleteOrganizacionlocalidad")]
        public ActionResult DeleteOrganizacionlocalidad([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                List<Organizacion_localidad> organizacion_localidadSave = JsonConvert.DeserializeObject<List<Organizacion_localidad>>(value.GetProperty("organizacionLocalidad").ToString());
                context.Organizacion_Localidad.RemoveRange(organizacion_localidadSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
    }
}
