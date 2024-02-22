using ApiRestCuestionario.Context;
using ApiRestCuestionario.Dto;
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

namespace ApiRestCuestionario.Controllers
{

    public class DeleteAnswerDto
    {
        [Required] 
        public int FormId { get; set; }
        [Required]
        public List<int> IdList { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        private readonly StaticFolder staticFolder;
        public AnswerController(AppDbContext context, StaticFolder staticFolder)
        {
            this.context = context;
            this.staticFolder = staticFolder;
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
                    List<string> documentsPath = new List<string>();
                    var formDocumentsPath = Path.Combine(staticFolder.Path, "Answers");
                    var userPath = Path.Combine(formDocumentsPath, form_id.ToString());
                    if (!Directory.Exists(userPath))
                    {
                        Directory.CreateDirectory(userPath);
                    }

                    foreach (var document in formDocument.file)
                    {
                        string filePath = Path.Combine(userPath, document.FileName);
                        Stream fileStream = new FileStream(filePath, FileMode.Create);
                        await document.CopyToAsync(fileStream);
                        documentsPath.Add($"{form_id}/{document.FileName}");
                    }
                    var answer = string.Join("|||", documentsPath);
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { answer, questions_id } });
                }
                else
                {
                    return StatusCode(400, new ItemResp { status = 404, message = "No hay archivos para guardar", data = null });

                }

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
                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_PRUEBA_RESPUESTA @formId = {answer.FormId} , @json = {answer.Data}");
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("SaveMasiveAnswer")]
        public async Task<ActionResult> SaveMasiveAnswer([FromBody] SaveMasiveAnswerDto answer)
        {
            try
            {
               var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_GUARDAR_RESPUESTAS @formId = {answer.FormId} , @json = {answer.Data}");
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

        [HttpDelete("DeleteMasiveAnswer")]
        public async Task<ActionResult> DeleteMasiveAnswer([FromBody] DeleteAnswerDto answerDto)
        {
            try
            {
                var itemsToDelete = string.Join(",", answerDto.IdList);
                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_ELIMINAR_RESPUESTAS @formId = {answerDto.FormId} , @items = {itemsToDelete}");
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
