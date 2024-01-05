using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        string OBTAIN = "Se cargo con éxito los datos";
        string CONFIRM = "Se creo con exito";
        string UPDATE = "Se Edito con exito";
        string UPDATELINKCANCEL = "Ya se ha publicado este formulario";
        string CONFIRMLINKSAVE = "Se publico este formulario correctamente";

        private readonly AppDbContext context;
        public FormsController(AppDbContext context)
        {
            this.context = context;
        }
        // GET: api/<FormsController>
        [HttpGet]
        public IEnumerable<Form> Get()
        {
            return context.Form.ToList();
        }

        // GET api/<FormsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        [Route("GetFormByIdUser")]
        public ActionResult GetFormByIdUser([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("users").GetProperty("users_id").ToString());
                object userForm = context.Form.Join(context.Users_Form, c => c.id, cm => cm.id, (c, cm) => new { form = c, userForm = cm }).Where(x => x.userForm.users_id == user_id).ToList();

                return StatusCode(200, new ItemResp { status = 200, message = OBTAIN, data = context.Form.ToList() });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetFormByLink")]
        public ActionResult GetFormByLink([FromBody] JsonElement value)
        {
            try
            {
                string link = JsonConvert.DeserializeObject<string>(value.GetProperty("form").GetProperty("link").ToString());
                object FormDataQuestions = context.Form.Join(context.Questions, c => c.id, cm => cm.form_id, (c, cm) => new { form = c, questions = cm }).Where(x => x.form.link.Equals(link)).Select(c => c.questions).OrderBy(c => c.position).ToList();
                object form_aparence = context.Form.Join(context.Form_Aparence, c => c.id, cm => cm.form_id, (c, cm) => new { form = c, formStyle = cm }).Where(x => x.form.link.Equals(link)).Select(c => c.formStyle).ToList();
                return StatusCode(200, new ItemResp { status = 200, message = OBTAIN, data = new  { questions = FormDataQuestions, aparence = form_aparence } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetFormByIdForm")]
        public ActionResult GetFormByIdForm([FromBody] JsonElement value)
        {
            try
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = context.Form.ToList() });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("EditFormLink")]
        public ActionResult EditFormLink([FromBody] JsonElement value)
        {
            try
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                string status = JsonConvert.DeserializeObject<string>(value.GetProperty("form").GetProperty("status").ToString());
                string link = JsonConvert.DeserializeObject<string>(value.GetProperty("form").GetProperty("link").ToString());

                Form editForm = context.Form.Where(c => c.id == form_id).First();
                if (editForm.link == null)
                {
                    editForm.link = link;
                    editForm.status = status;
                    context.Form.Update(editForm);
                    context.SaveChanges();
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRMLINKSAVE, data = editForm });
                }
                else
                {
                    return StatusCode(200, new ItemResp { status = 200, message = UPDATELINKCANCEL, data = editForm });

                }

            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        // POST api/<FormsController>
        [HttpPost]
        public async Task<ActionResult<ItemResponse>> Post([FromBody] JsonElement value)
        {
            try
            {
                //Primeramente se crea un nuevo formulario
                Form form = JsonConvert.DeserializeObject<Form>(value.GetProperty("form").ToString());
                int idUser = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                int proy_id = value.GetProperty("proyecto").GetProperty("id_proyecto").GetInt32();

                // Crea el parámetro de salida
                var outputIdParam = new SqlParameter
                {
                    ParameterName = "@OutputId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output // Especifica que es un parámetro de salida
                };

                await context.Database
              .ExecuteSqlInterpolatedAsync($@"exec sp_guardar_encuesta
                    @encuestaid={form.id},
                    @nombreencuesta={form.form_name},
                    @NombreEncuestaDB={form.form_db_name},
                    @labelencuesta={form.form_label},
                    @resumenencuesta={form.form_abstract},
                    @idproyecto={proy_id},
                    @iduser={idUser},
                    @OutputId ={outputIdParam} OUTPUT");

                var outputId = (int)outputIdParam.Value;
                form.id = outputId;
                //if (form.id != 0) {
                //    context.Form.Update(form);
                //    context.SaveChanges();
                //}
                //else
                //{
                //    context.Form.Add(form);
                //    context.SaveChanges();
                //    //Luego se reserva el último id
                //    int lastFormId = form.id;
                //    Users_Form users_form = new Users_Form(idUser, lastFormId,"1");
                //    //Se guarda Users_Form
                //    context.Users_Form.Add(users_form);
                //    context.SaveChanges();
                //}

                return StatusCode(200, new ItemResp { message = CONFIRM, status = 200, data = form });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        // PUT api/<FormsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FormsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}