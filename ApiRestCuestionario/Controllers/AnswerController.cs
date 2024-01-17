using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    public class SaveAnswerDTO
    {
        [Required]
        public int formId { get; set; }
        [Required]
        public List<Answers> dataAnswer { get; set; }
        [Required]
        public List<AnswerAnioMes> listDataAnioMes { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public AnswerController(AppDbContext context)
        {
            this.context = context;
        }
        // GET: api/<AnswerController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AnswerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        [Route("SaveDocuments")]
        public async Task<ActionResult> SaveDocumentsAsync([FromForm] FormFilecs form)
        {
            try
            {
                int form_id = int.Parse(JsonConvert.DeserializeObject<string>(form.formId));
                int questions_id = int.Parse(JsonConvert.DeserializeObject<string>(form.questionsId));
                List<string> joinToPathDocument = new List<string>();
                foreach (IFormFile document in form.file)
                {
                    //Si no existe que cree la carpeta DocumentsAnswers
                    //Direccion total seria : CuestionarioRepo\Encuestas_Back2\Encuestas_Back\ApiRestCuestionario\bin\Debug\netcoreapp3.1\DocumentsAnswers
                    //El numero significa el id del formularios
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsAnswers");
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "DocumentsAnswers\\" + form_id.ToString() + "\\", document.FileName);
                    joinToPathDocument.Add(filePath);
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsAnswers\\" + form_id.ToString());
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(fileStream);
                    }
                }
              
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { answer = string.Join("|||", joinToPathDocument), questions_id } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("SaveArchiveForm")]
        public async Task<ActionResult> SaveArchiveForm([FromForm] FormFilecs form)
        {
            try
            {
                int form_id = int.Parse(JsonConvert.DeserializeObject<string>(form.formId));
                string filePath = "";
                //Se junta las direcciones de guardado en un string
                List<string> joinToPathDocument = new List<string>();
                foreach (IFormFile document in form.file)
                {
                    //Si no existe que cree la carpeta DocumentsAnswers
                    //Direccion total seria : CuestionarioRepo\Encuestas_Back2\Encuestas_Back\ApiRestCuestionario\bin\Debug\netcoreapp3.1\DocumentsAnswers
                    //El numero significa el id del formularios
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsArchiveForm");
                    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "DocumentsArchiveForm\\" + form_id.ToString() + "\\", document.FileName);
                    joinToPathDocument.Add(filePath);
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsArchiveForm\\" + form_id.ToString());
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(fileStream);
                    }
                }
                Form formEdit = context.Form.ToList().Where(c => c.id == form_id).FirstOrDefault();
                formEdit.archive = filePath;
                context.Form.Update(formEdit);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = filePath });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetArchiveForm")]
        public ActionResult GetArchiveForm([FromBody] JsonElement form)
        {
            try
            {
                string filePath = JsonConvert.DeserializeObject<string>(form.GetProperty("path").ToString());
                byte[] archivoBytes = System.IO.File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(archivoBytes);
                //Se junta las direcciones de guardado en un string

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = base64 });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost]
        [Route("SaveAnswer")]
        public async Task<ActionResult> SaveAnswer([FromBody] SaveAnswerDTO answer)
        {
            try
            {
                Guid uuidV4 = Guid.NewGuid();

                DateTime fechasave = DateTime.Now;
                foreach (Answers ans in answer.dataAnswer)
                {
                    ans.hashUnic = uuidV4.ToString();
                    ans.answer_date = fechasave;
                }
                foreach (AnswerAnioMes ansa in answer.listDataAnioMes)
                {
                    ansa.hashUnic = uuidV4.ToString();
                    ansa.answer_date = fechasave;
                }
                context.Answers.AddRange(answer.dataAnswer);
                context.AnswerAnioMes.AddRange(answer.listDataAnioMes);
                context.SaveChanges();
                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_GUARDAR_RESPUESTA_FORMULARIO @form_id = {answer.formId} , @json = {JsonConvert.SerializeObject(answer.dataAnswer)}");
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("SaveMasiveAnswer")]
        public ActionResult SaveMasiveAnswer([FromBody] JsonElement form)
        {
            try
            {
                DateTime fechasaveGeneral = DateTime.Now;
                List<List<Answers>> answersSave = JsonConvert.DeserializeObject<List<List<Answers>>>(form.GetProperty("dataAnswer").ToString());

                List<Answers> saveAnswersToBd = new List<Answers>();
                List<AnswerAnioMes> answersAnioSaveSave = JsonConvert.DeserializeObject<List<AnswerAnioMes>>(form.GetProperty("listDataAnioMes").ToString());



                foreach (var ans in answersSave)
                {
                    DateTime fechasave = DateTime.Now;
                    foreach (var c in ans)
                    {
                        c.answer_date = fechasave;
                    }
                    context.Answers.AddRange(ans);
                }

                foreach (AnswerAnioMes ansa in answersAnioSaveSave)
                {
                    ansa.answer_date = fechasaveGeneral;
                }
                //context.Answers.AddRange(saveAnswersToBd);
                context.AnswerAnioMes.AddRange(answersAnioSaveSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetAnswerAnioMesByIdForm")]
        public ActionResult GetAnswerAnioMesByHashUnic([FromBody] JsonElement value)
        {
            try
            {

                string idform = JsonConvert.DeserializeObject<string>(value.GetProperty("form").GetProperty("form_id").ToString());

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = context.AnswerAnioMes.ToList().Where(c => c.hashUnic == idform) });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("DeleteAnswer")]
        public ActionResult DeleteAnswer([FromBody] JsonElement form)
        {
            try
            {
                List<Answers> answersDelete = JsonConvert.DeserializeObject<List<Answers>>(form.GetProperty("answer").ToString());
                foreach (Answers ansa in answersDelete)
                {
                    ansa.Flg_proceso = "4";
                }
                context.Answers.UpdateRange(answersDelete);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetAnswerAnioMesByIdFormReal")]
        public ActionResult GetAnswerAnioMesByIdForm([FromBody] JsonElement value)
        {
            try
            {

                int idform = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = context.AnswerAnioMes.ToList().Where(c => c.idForm == idform) });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        // POST api/<AnswerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AnswerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AnswerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
