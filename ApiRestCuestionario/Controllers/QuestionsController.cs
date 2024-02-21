using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{

    public class SaveQuestionDTO
    {
        public int formId { get; set; }
        public Form_Aparence? aparence { get; set; }
        public List<Quest> questions { get; set; }
    }
    public class Quest
    {
        public int? id { get; set; }
        public string? column_type { get; set; }
        public string? column_type_2 { get; set; }
        public string? column_name { get; set; }
        public string? column_db_name { get; set; }
        public string? column_db_name_2 { get; set; }
        public int question_type_id { get; set; }
        public string? props_ui { get; set; }
        public bool? deleted { get; set; }
        public bool hidden { get; set; }
    }

    public class ColumnInfo
    {
        public int? id { get; set; }
        public string? columnName { get; set; }
        public string? columnDBName { get; set; }
        public string? columnType { get; set; }
        public JObject? props_ui { get; set; }
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
                var questions = context.column_types.Where(c => c.form_id == formId && c.props_ui != null);
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

            List<string> columns = context.column_types.Where(x => x.form_id == formId).Select(x => x.nombre_columna_db).ToList();
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
                //List<string> toUpdateColumns = new List<string>();
                //foreach (var x in toUpdate)
                //{
                //    var normalized = StringParser.NormalizeString(x.column_db_name);
                //    var parsedColumn = "";
                //    if (!toUpdateColumns.Contains(normalized))
                //    {
                //        toUpdateColumns.Append(normalized);
                //        parsedColumn = normalized;
                //    }
                //    else if (itemsCounter.ContainsKey(normalized))
                //    {
                //        itemsCounter[normalized]++;
                //        parsedColumn = $"{normalized}_{itemsCounter[normalized]}";
                //    }
                //    else
                //    {
                //        itemsCounter[normalized] = 1;
                //        parsedColumn = normalized;
                //    }
                //    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_UPDATE_STATE_PROPS @Id={x.id}, @NuevoEstado={(x.hidden ? 0 : 1)};");

                //    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_ACTUALIZAR_COLUMNA @idColumn={x.id}, @columnName={x.column_name}, @columnNameDB={parsedColumn}, @dataType={x.column_type}, @propsUi = {x.props_ui};");

                //}

                List<string> toUpdateColumns = new List<string>();
                foreach (var x in toUpdate)
                {
                    // Normalización y manejo de column_db_name
                    var normalized = StringParser.NormalizeString(x.column_db_name);
                    var parsedColumn = "";
                    if (!toUpdateColumns.Contains(normalized))
                    {
                        toUpdateColumns.Add(normalized); // Usa Add en lugar de Append para List
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

                    // Manejo de column_db_name_2, similar a column_db_name pero con control de null
                    var parsedColumn2 = "";
                    if (x.column_db_name_2 != null)
                    {
                        var normalized2 = StringParser.NormalizeString(x.column_db_name_2);
                        if (!toUpdateColumns.Contains(normalized2))
                        {
                            toUpdateColumns.Add(normalized2);
                            parsedColumn2 = normalized2;
                        }
                        else if (itemsCounter.ContainsKey(normalized2))
                        {
                            itemsCounter[normalized2]++;
                            parsedColumn2 = $"{normalized2}_{itemsCounter[normalized2]}";
                        }
                        else
                        {
                            itemsCounter[normalized2] = 1;
                            parsedColumn2 = normalized2;
                        }
                    }
                    else
                    {
                        parsedColumn2 = null;
                    }

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_UPDATE_STATE_PROPS @Id={x.id}, @NuevoEstado={(x.hidden ? 0 : 1)};");

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_UPDATE_COLUMN_2 @idColumn={x.id}, @columnName={x.column_name}, @columnNameDB={parsedColumn}, @dataType={x.column_type}, @columnNameDB2={parsedColumn2}, @dataType2={x.column_type_2}, @propsUi = {x.props_ui};");
                }

            }
            if (toInsert.Any())
            {

                var insertList = toInsert.Select(x =>
                {
                    // Normalización y generación del nombre de la primera columna de base de datos
                    var normalizedColumnName = StringParser.NormalizeString(x.column_db_name);
                    var columnNameDB = normalizedColumnName;
                    if (itemsCounter.ContainsKey(normalizedColumnName))
                    {
                        itemsCounter[normalizedColumnName]++;
                        columnNameDB = $"{normalizedColumnName}_{itemsCounter[normalizedColumnName]}";
                    }
                    else
                    {
                        itemsCounter[normalizedColumnName] = 1;
                    }

                    // Normalización y generación del nombre de la segunda columna de base de datos, si aplica
                    var columnNameDB2 = x.column_db_name_2 != null ? StringParser.NormalizeString(x.column_db_name_2) : null;
                    if (columnNameDB2 != null && itemsCounter.ContainsKey(columnNameDB2))
                    {
                        itemsCounter[columnNameDB2]++;
                        columnNameDB2 = $"{columnNameDB2}_{itemsCounter[columnNameDB2]}";
                    }
                    else if (columnNameDB2 != null)
                    {
                        itemsCounter[columnNameDB2] = 1;
                    }

                    // Ajuste para manejar column_type_2 cuando no es null
                    var columnType2 = x.column_type_2 != null ? $"{x.column_type_2}" : null; // Ajuste según la lógica de formateo si es necesario

                    return new
                    {
                        columnName = x.column_name,
                        columnType = x.column_type,
                        columnType2 = columnType2, // Incluido columnType2
                        propsUi = x.props_ui,
                        formId = formId,
                        state = 1,
                        questionTypeId = x.question_type_id,
                        columnNameDB, // Calculated as above
                        columnNameDB2 // Incluido columnNameDB2
                    };
                }).ToList();

                var jsonParameter = JsonConvert.SerializeObject(insertList);
                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_INSERT_COLUMNS @jsonInput={jsonParameter}, @formId={formId};");
            }
            return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { questionDTO } });

        }
        [HttpPost("UpdateQuestionState")]
        public async Task<ActionResult> UpdateQuestionState(int questionId, int newState)
        {
            try
            {
                // Llama al procedimiento almacenado spActualizarEstado
                await context.Database.ExecuteSqlInterpolatedAsync($"EXEC spActualizarEstado @Id={questionId}, @NuevoEstado={newState}");

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { QuestionId = questionId, NewState = newState } });
            }
            catch (Exception ex)
            {
                // Maneja la excepción
                return BadRequest(ex.Message);
            }
        }

    

        [HttpGet("types")]
        public async Task<ActionResult> GetQuestionTypes()
        {
            var data = await context.question_types.ToListAsync();
            return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos con éxito", data = data });
        }

    }
}
