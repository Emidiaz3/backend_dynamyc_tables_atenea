using angular.Server.Model;
using ApiRestCuestionario.Context;
using ApiRestCuestionario.Dto;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace ApiRestCuestionario.Controllers
{
    public class ArchiveForm
    {
        public string? path { get; set; }
    }
    public class AnswerAnioMesByHashUnic
    {
        public CustomForm? Form { get; set; }
    }

    public class CustomForm
    {
        public string? form_id { get; set; }
    }

    public class AnswerDelete
    {
        public List<Answers>? answer {  get; set; }
    }

    public class DeleteAnswerDto
    {
        [Required]
        public int FormId { get; set; }
        [Required]
        public List<int> IdList { get; set; } = [];
    }
    public class DeleteAnswerCorrelDTO
    {
        [Required]
        public int FormId { get; set; }
        [Required]
        public List<string> Correlativos { get; set; } = [];
    }
    public class AnswerDTO
    {
        public string? Answer { get; set; }
        public DateTime? AnswerDate { get; set; }
        public int UsersId { get; set; }
        public int FormId { get; set; }
        public string? FlgProceso { get; set; }
        public int QuestionsId { get; set; }
        public string? DbName { get; set; }
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
                    string db_name = formDocument.db_name;
                    List<string> documentsPath = [];
                    var formDocumentsPath = Path.Combine(staticFolder.Path, "Answers");
                    var userPath = Path.Combine(formDocumentsPath, form_id.ToString());
                    if (!Directory.Exists(userPath))
                    {
                        Directory.CreateDirectory(userPath);
                    }
                    foreach (var document in formDocument.file)
                    {
                        string filePath = Path.Combine(userPath, document.FileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await document.CopyToAsync(fileStream);
                        }
                        documentsPath.Add($"{form_id}/{document.FileName}");
                    }

                    var answer = string.Join("|||", documentsPath);
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { answer, questions_id, db_name } });
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
                Form? formEdit = context.Form.ToList().Where(c => c.id == form_id).FirstOrDefault();
                if (formEdit != null)
                {
                    formEdit.archive = filePath;
                    context.Form.Update(formEdit);
                    context.SaveChanges();

                }

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = filePath });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("GetArchiveForm")]
        public ActionResult GetArchiveForm([FromBody] ArchiveForm form)
        {
            try
            {
                string filePath = form.path!;
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
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_PRUEBA_RESPUESTA @formId = {answer.FormId} , @json = {answer.Data}, @user={userName}");
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
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_GUARDAR_RESPUESTAS @formId = {answer.FormId} , @json = {answer.Data},@user={userName}");
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost("SaveAnswerForm")]
        public async Task<ActionResult> SaveAnswerForm([FromBody] SaveMasiveAnswerDto answer)
        {
            try
            {
                var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC [SP_REGISTRAR_FORMULARIO] @formId = {answer.FormId} , @json = {answer.Data},@user={userName}");
                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC [dbo].[SP_REGISTRAR_FORMULARIO_MASIVO] @formId = {answer.FormId} , @jsons = {answer.Data},@user={userName}");
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("GetAnswerAnioMesByIdForm")]
        public ActionResult GetAnswerAnioMesByHashUnic([FromBody] AnswerAnioMesByHashUnic value)
        {
            try
            {
                string idform = value.Form!.form_id!;
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = context.AnswerAnioMes.ToList().Where(c => c.hashUnic == idform) });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost("DeleteAnswer")]
        public ActionResult DeleteAnswer([FromBody] AnswerDelete form)
        {
            try
            {
                List<Answers> answersDelete = form.answer!;
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

        [HttpPost("DeleteMasiveAnswer")]
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

        [HttpPost("DeleteAnswersByCorrel")]
        public async Task<ActionResult> DeleteAnswersByCorrel([FromBody] DeleteAnswerCorrelDTO answerDto)
        {
            try
            {
                var itemsToDelete = string.Join(",", answerDto.Correlativos);
                var response = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC [SP_ELIMINAR_RESPUESTAS_V2] @formId = {answerDto.FormId} , @correlativos = {itemsToDelete}");
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
