using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ApiRestCuestionario.Controllers
{

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
        public int question_type_id { get; set; }
        public string props_ui { get; set; }
        public bool? deleted { get; set; }
    }

    public class ColumnInfo
    {
        public int? id { get; set; }
        public string columnName { get; set; }
        public string columnDBName { get; set; }
        public string columnType { get; set; }
        public JObject props_ui { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
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
                var questions = context.column_types.Where(c => c.form_id == formId && c.props_ui != null && c.state == 1);
                var aparence = context.Form_Aparence.FirstOrDefault(c => c.form_id == formId);
                var form = context.Form.Where(c => c.id == formId).FirstOrDefault();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { aparence, questions, form } });
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
            var itemsCounter = StringParser.CheckColumnItems(columns);
            context.Form_Aparence.Add(aparenceSave);
            if (aparenceSave.id != 0)
            {
                context.Form_Aparence.Update(aparenceSave);
            }
            context.SaveChanges();
            var toDelete = questions.Where(x => x.deleted == true && x.id != null);
            var toInsert = questions.Where(x => x.id == null);
            var toUpdate = questions.Where(x => x.deleted != true && x.id != null);

            if (toDelete.Count() != 0)
            {
                foreach (var questionD in toDelete)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($"Exec dbo.UpdateStateById @id={questionD.id}, @newState={0}");
                }
            }
            if (toUpdate.Any())
            {
                List<string> toUpdateColumns = new List<string>();
                foreach (var x in toUpdate)
                {
                    var normalized = StringParser.NormalizeString(x.column_db_name);
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
            if (toInsert.Any())
            {
                var columnNames = string.Join(",", toInsert.Select(x => x.column_name));
                var questionId = string.Join(",", toInsert.Select(x => x.question_type_id));
                var columnNamesDB = string.Join(",", toInsert.Select(x =>
                {
                    var item = StringParser.NormalizeString(x.column_db_name);
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
                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC AddColumnsAndInsertData @columnNames={columnNames}, @columnNamesDB={columnNamesDB}, @columnTypes={columnTypes}, @props_ui = {props_ui}, @formId={formId}, @questionTypesId = {questionId};");
            }
            return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { questionDTO } });

        }



        [HttpGet("types")]
        public async Task<ActionResult> GetQuestionTypes()
        {
            var data = await context.question_types.ToListAsync();
            return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos con éxito", data = data });
        }


    }
}
