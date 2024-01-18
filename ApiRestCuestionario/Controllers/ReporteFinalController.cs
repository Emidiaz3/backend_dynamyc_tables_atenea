using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    public class SaveDocumentDTO
    {
        public int form_id { get; set; }
        public int user_id { get; set; }
        public IFormFile file { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class ReporteFinalController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        //private readonly IConfiguration config;
        public ReporteFinalController(AppDbContext context)// IConfiguration _config
        {
            this.context = context;
            //this.config = _config;
        }
        [HttpGet]
        [Route("GetReporteFinal")]
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
        [HttpPost]
        [Route("GetReportByAnioMes")]
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
        [HttpPost]
        [Route("DeleteReportByAnioMes")]
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

        public async Task<ActionResult> SaveClientDocument([FromForm, BindRequired] SaveDocumentDTO documentDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Console.WriteLine("gei");
                }
                Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                string fileName = documentDTO.file.FileName;
                string reportsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
                string clientDirectory = Path.Combine(reportsDirectory, documentDTO.user_id.ToString());
                string filePath = Path.Combine(clientDirectory, fileName);
                if (!Directory.Exists(reportsDirectory))
                {
                    Directory.CreateDirectory(reportsDirectory);
                }

                if (!Directory.Exists(clientDirectory))
                {
                    Directory.CreateDirectory(clientDirectory);
                }
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await documentDTO.file.CopyToAsync(fileStream);

                }
                context.documents.Add(new Documents { name = fileName, file_path = filePath, form_id = documentDTO.form_id, user_id = documentDTO.user_id });
                await context.SaveChangesAsync();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = "Document Guardado Correctamente" });

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return StatusCode(400, new ItemResp { status = 500, message = CONFIRM, data = "Fallo Al guardar" });
            }
        }

        [HttpGet("GetReportDocuments")]
        public async Task<ActionResult> getReportDocuments([FromQuery][Required] int formId, [FromQuery] int userId)
        {
            var query = from document in context.documents join user in context.t_mae_usuario on document.user_id equals user.IdUsuario where document.user_id == userId && document.form_id == formId  select new { user, document };
            var items = await query.ToListAsync();

            return StatusCode(200, new ItemResp { status = 400, message = CONFIRM, data = items });

        }
    }
}
