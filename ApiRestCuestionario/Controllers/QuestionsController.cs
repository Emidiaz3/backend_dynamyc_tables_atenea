using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ApiRestCuestionario.Controllers
{
    public class Utils
    {
        public static string NormalizeString(string str)
        {
            Console.WriteLine(str);
            string normalizedString = str.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Trim().ToLower().Replace(" ", "_");
        }
        public static Dictionary<string, int> CheckColumnItems(List<string> columns)
        {
            Dictionary<string, int> itemsCounter = new Dictionary<string, int>();

            foreach (var x in columns)
            {
                var items = x.Trim().Split("_");
                var isNumeric = int.TryParse(items.Last(), out int n);

                if (items.Length > 1 && isNumeric)
                {
                    var verificationString = string.Join("_", items.Take(items.Length - 1));
                    if (itemsCounter.ContainsKey(verificationString) && n > itemsCounter[verificationString])
                    {
                        itemsCounter[verificationString] = n;
                    }
                    else
                    {
                        itemsCounter[x] = 1;
                    }
                }
                else
                {
                    itemsCounter[x] = 1;
                }
            }
            return itemsCounter;
        }


    }
    public class SaveQuestionDTO
    {
        public int formId { get; set; }
        public Form_Aparence aparence { get; set; }
        public List<Quest> questions { get; set; }
    }
    public class Quest
    {
        public int? id { get; set; }
        public string column_type { get; set; }
        public string column_name { get; set; }
        public string column_db_name { get; set; }
        public string props_ui { get; set; }
        public bool? deleted { get; set; }
    }

    public class ColumnInfo
    {
        public int id { get; set; }
        public string columnName { get; set; }
        public string columnDBName { get; set; }
        public string columnType { get; set; }
        public JObject props_ui { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public QuestionsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult GetQuestions([FromQuery][Required] int formId)
        {
            try
            {
                object questions = context.column_types.Where(c => c.form_id == formId && c.props_ui != null && c.state == 1);
                object aparence = context.Form_Aparence.FirstOrDefault(c => c.form_id == formId);
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new  {aparence,questions } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> SaveQuestions([FromBody] SaveQuestionDTO questionDTO)
        {
            var aparenceSave = questionDTO.aparence;
            var questions = questionDTO.questions;
            int formId = questionDTO.formId;
            var columns = context.column_types.Where(x => x.form_id == formId).Select(x => x.nombre_columna_db).ToList();
            var itemsCounter = Utils.CheckColumnItems(columns);
            context.Form_Aparence.Add(aparenceSave);
            if (aparenceSave.id != 0)
            {
                context.Form_Aparence.Update(aparenceSave);
            }
            context.SaveChanges();
            // Empieza Eliminado
            var toDelete = questions.Where(x => x.deleted==true && x.id != null);
            var toInsert = questions.Where(x => x.id == null);
            var toUpdate = questions.Where(x => x.deleted != true && x.id != null);

            if(toDelete.Count() != 0)
            {
                foreach (var questionD in toDelete)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($"Exec dbo.UpdateStateById @id={questionD.id}, @newState={0}");
                }
            }
            if (toUpdate.Count() != 0)
            {
                List<string> toUpdateColumns = new List<string>();
                foreach (var x in toUpdate)
                {
                    var normalized = Utils.NormalizeString(x.column_db_name);
                    var parsedColumn = "";
                    if (!toUpdateColumns.Contains(normalized))
                    {
                        toUpdateColumns.Append(normalized);
                        parsedColumn = normalized;
                    }
                    else if (itemsCounter.ContainsKey(normalized))
                    {
                        itemsCounter[normalized]++;
                        parsedColumn = $"{normalized}_{itemsCounter[normalized]}";
                    }
                    else
                    {
                        itemsCounter[normalized] = 1;
                        parsedColumn = normalized;
                    }

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_UPDATE_COLUMN @idColumn={x.id}, @columnName={x.column_name}, @columnNameDB={parsedColumn}, @dataType={x.column_type}, @propsUi = {x.props_ui};");

                }

            }
            if (toInsert.Count() != 0)
            {
                var columnNames = string.Join(",", toInsert.Select(x => x.column_name));
                var columnNamesDB = string.Join(",", toInsert.Select(x =>
                {
                    var item = Utils.NormalizeString(x.column_db_name);
                    if (itemsCounter.ContainsKey(item))
                    {
                        itemsCounter[item]++;
                        return $"{item}_{itemsCounter[item]}";
                    }
                    else
                    {
                        itemsCounter[item] = 1;
                        return item;
                    }
                }));
                var columnTypes = string.Join(",", toInsert.Select(x => x.column_type));
                var props_ui = JsonConvert.SerializeObject(toInsert.Select(x => x.props_ui));
                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC AddColumnsAndInsertData @columnNames={columnNames}, @columnNamesDB={columnNamesDB}, @columnTypes={columnTypes}, @props_ui = {props_ui}, @formId={formId};");
            }

           
            return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { questionDTO }  });

        }

       
        [HttpPost("UpdateColumns")]
        public async Task<ActionResult> UpdateColumns([FromBody] JsonElement value)
        {
            try
            {
                var questionsRoot = JsonConvert.DeserializeObject<List<ColumnInfo>>(value.GetProperty("questions").GetRawText());

                // delete state = 0

                foreach (var question in questionsRoot)
                {
                    string propsUiJsonE = JToken.FromObject(question.props_ui).ToString(Formatting.None);

                    if (question.id == null)
                    {
                        // insert
                    }                    // Si necesitas convertirlo en un string, puedes hacerlo aquí
                    else
                    {
                        if (question.id != null)
                        {
                            var result = await context.Database.ExecuteSqlInterpolatedAsync($"Exec [dbo].[SP_UPDATE_COLUMN] @idColumn={question.id}, @columnName={question.columnName}, @columnNameDB={question.columnDBName},@dataType={question.columnType},@propsUi={propsUiJsonE}");

                        }
                    }
                }
                return Ok(new { status = 200, message = "Verificación completada." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }
        }

       
        [HttpGet("types")]
        public async Task<ActionResult> GetQuestionTypes()
        {
            var data = await context.question_types.ToListAsync();
          
            return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos con éxito", data=data });
        }


        [HttpGet("CheckColumnNames")]
        public async Task<ActionResult> CheckColumnNames([FromBody] JsonElement value)
        {
            try
            {
                string columnNames = value.GetProperty("columnNames").ToString();
                int idEncuesta = value.GetProperty("idEncuesta").GetInt32();
                var result = await context.Database.ExecuteSqlInterpolatedAsync($"Exec [dbo].[SP_CHECK_COLUMN_NAMES] @stringArray ={columnNames}, @idEncuesta ={idEncuesta}");
                // implementar verificación de truncamiento solo se envia los objetos del JSON con id != null, es decir los ke se van a actualizar porke podrian tener datos.
                return Ok(new { status = 200, message = "Verificación completada." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }
        }
    }
}
