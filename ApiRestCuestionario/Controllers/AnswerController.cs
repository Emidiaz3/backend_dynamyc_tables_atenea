using ApiRestCuestionario.Context;
using ApiRestCuestionario.Dto;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public AnswerController(AppDbContext context)
        {
            this.context = context;
        }
   
        [HttpPost("SaveDocuments")]
        public async Task<ActionResult> SaveDocuments([FromForm] SaveFormDocumentDto formDocument)
        {
            try
            {
                if (formDocument.file.Any())
                {
                    int form_id = formDocument.formId;
                    int questions_id = formDocument.questionsId;
                    List<string> joinToPathDocument = [];
                    foreach (var document in formDocument.file)
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsAnswers");
                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "DocumentsAnswers\\" + form_id.ToString() + "\\", document.FileName);
                        joinToPathDocument.Add(filePath);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsAnswers\\" + form_id.ToString());
                        using Stream fileStream = new FileStream(filePath, FileMode.Create);
                        await document.CopyToAsync(fileStream);
                    }
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { answer = string.Join("|||", joinToPathDocument), questions_id } });
                }

                return StatusCode(400, new ItemResp { status = 404, message = "No hay archivos para guardar", data = null });

            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("SaveArchiveForm")]
        public async Task<ActionResult> SaveArchiveForm([FromForm] SaveFormDocumentDto formDocument)
        {
            try
            {
                int form_id = formDocument.formId;
                string filePath = "";
                List<string> joinToPathDocument = new List<string>();
                foreach (IFormFile document in formDocument.file)
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsArchiveForm");
                    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "DocumentsArchiveForm\\" + form_id.ToString() + "\\", document.FileName);
                    joinToPathDocument.Add(filePath);
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DocumentsArchiveForm\\" + form_id.ToString());
                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await document.CopyToAsync(fileStream);
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

        [HttpPost("GetArchiveForm")]
        public ActionResult GetArchiveForm([FromBody] JsonElement form)
        {
            try
            {
                string filePath = JsonConvert.DeserializeObject<string>(form.GetProperty("path").ToString());
                byte[] archivoBytes = System.IO.File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(archivoBytes);
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = base64 });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("SaveAnswer")]
        public async Task<ActionResult> SaveAnswer([FromBody] SaveAnswerDTO answer)
        {
            try
            {
                var serializedAnswers = JsonConvert.SerializeObject(answer.dataAnswer);
                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_GUARDAR_RESPUESTA_FORMULARIO @form_id = {answer.formId} , @json = {serializedAnswers}");
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("SaveMasiveAnswer")]
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
                context.AnswerAnioMes.AddRange(answersAnioSaveSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("GetAnswerAnioMesByIdForm")]
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

        [HttpPost("DeleteAnswer")]
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

        [HttpPost("GetAnswerAnioMesByIdFormReal")]
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

    }
}
