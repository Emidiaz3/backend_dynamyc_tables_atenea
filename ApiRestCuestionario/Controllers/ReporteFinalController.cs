using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    public class SaveDocumentDTO
    {
        [Required]
        public int? formId { get; set; }
        [Required]
        public int? userId { get; set; }
        [Required]
        public List<IFormFile> Files { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ReporteFinalController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        private readonly StaticFolder staticFolder;
        public ReporteFinalController(AppDbContext context, StaticFolder staticFolder)
        {
            this.context = context;
            this.staticFolder = staticFolder;
        }

        [HttpGet("GetReporteFinal")]
        public ActionResult GetReporteFinal()
        {
            var response = new ItemResponse();
            try
            {
                var ReporteFinaldata = context.ReporteFinalL.FromSqlInterpolated($"Exec SP_REPORTE_FINAL ").AsAsyncEnumerable();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ReporteFinaldata });
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }
                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }
        [HttpPost("GetReportByAnioMes")]
        public ActionResult GetReportByAnioMes([FromBody] JsonElement value)
        {
            var response = new ItemResponse();

            try
            {
                ReporteFinal listDatosLaboratorio = JsonConvert.DeserializeObject<ReporteFinal>(value.GetProperty("dataAnioMes").ToString());

                var ReporteFinaldata = context.ReporteFinal.FromSqlInterpolated($"Exec SP_REPORTE_FINAL_SELECT_BY_MES_ANIO @anio={listDatosLaboratorio.Año} ,@mes ={listDatosLaboratorio.Numes}").AsAsyncEnumerable(); ;
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ReporteFinaldata });
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }
        [HttpPost("DeleteReportByAnioMes")]
        public ActionResult DeleteReportByAnioMes([FromBody] JsonElement value)
        {
            var response = new ItemResponse();

            try
            {
                ReporteFinal listDatosLaboratorio = JsonConvert.DeserializeObject<ReporteFinal>(value.GetProperty("dataAnioMes").ToString());
                List<ReporteFinalDetail> lisdelete = context.ReporteFinal.Where(c => c.Numes == listDatosLaboratorio.Numes).Where(c => c.Año == listDatosLaboratorio.Año).ToList();

                context.ReporteFinal.RemoveRange(lisdelete);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }


        [HttpPost("SaveClientDocument")]
        public async Task<ActionResult> SaveClientDocument([FromForm] SaveDocumentDTO documentDTO)
        {
            try
            {
                if (documentDTO.Files.Any() && documentDTO.userId != null && documentDTO.formId != null)
                {
                    int userId = (int) documentDTO.userId;
                    int formId = (int) documentDTO.formId;
                    string reportsDirectory = Path.Combine(staticFolder.Path, "Reports");
                    string clientDirectory = Path.Combine(reportsDirectory, userId.ToString());
                    List<Documents> DocumentRange = new List<Documents>();
                    if (!Directory.Exists(reportsDirectory))
                    {
                        Directory.CreateDirectory(reportsDirectory);
                    }
                    if (!Directory.Exists(clientDirectory))
                    {
                        Directory.CreateDirectory(clientDirectory);
                    }
                    foreach (IFormFile file in documentDTO.Files)
                    {
                        string fileName = file.FileName;
                        string filePath = Path.Combine(clientDirectory, fileName);
                        Stream fileStream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                        DocumentRange.Add(new Documents { name = fileName, file_path = $"{userId}/{fileName}", form_id = formId, user_id = userId });
                    }
                    context.documents.AddRange(DocumentRange);
                    await context.SaveChangesAsync();
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = "Document Guardado Correctamente" });
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(400, new ItemResp { status = 400, message = CONFIRM, data = "Fallo Al guardar" });
            }
        }

        [HttpGet("GetReportDocuments")]
        public async Task<ActionResult> getReportDocuments([FromQuery][Required] int formId)
        {
            var query = from document in context.documents join user in context.t_mae_usuario on document.user_id equals user.IdUsuario where document.form_id == formId  select new { user, document };
            var items = await query.ToListAsync();
            return StatusCode(200, new ItemResp { status = 400, message = CONFIRM, data = items });
        }
    }
}
