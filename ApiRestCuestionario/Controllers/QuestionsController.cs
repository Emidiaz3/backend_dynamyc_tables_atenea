using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{

    public class SaveQuestionDTO
    {
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

        [HttpPost("{formId}")]
        public async Task<ActionResult> SaveQuestions([FromRoute] int formId, [FromBody] SaveQuestionDTO questionDTO)
        {
            var aparenceSave = questionDTO.aparence;
            var questions = questionDTO.questions;

            List<string> columnsDB1 = context.column_types
                .Where(x => x.form_id == formId)
                .Select(x => x.nombre_columna_db)
                .ToList();

            List<string> columnsDB2 = context.column_types
                .Where(x => x.form_id == formId)
                .Select(x => x.nombre_columna_db_2)
                .Where(x => x != null) 
                .ToList();

            List<string> allColumns = columnsDB1.Union(columnsDB2).ToList();
            var itemsCounter = StringParser.CheckColumnItems(allColumns);


            context.Form_Aparence.Add(aparenceSave);
            if (aparenceSave.id != 0)
            {
                context.Form_Aparence.Update(aparenceSave);
            }
            context.SaveChanges();

            var toDelete = questions.Where(x => x.deleted == true && x.id != null);
            var toInsert = questions.Where(x => x.id == null);
            var toUpdate = questions.Where(x => x.deleted != true && x.id != null);

            if (toDelete.Any())
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
                    var currentColumnObject = context.column_types.FirstOrDefault(y => y.Id == x.id);

                    if (currentColumnObject != null)
                    {
                        var currentColDB = currentColumnObject.nombre_columna_db;
                        var currentColDB2 = currentColumnObject.nombre_columna_db_2;

                        var newColDB = x.column_db_name;
                        var newColDB2 = x.column_db_name_2;

                        // Verifica si el nombre de la columna db ha cambiado y no es null o vacío
                        if (!string.IsNullOrWhiteSpace(newColDB) && !newColDB.Equals(currentColDB, StringComparison.OrdinalIgnoreCase))
                        {
                            toUpdateColumns.Add(newColDB);
                        }

                        // Verifica si el nombre de la columna db2 ha cambiado y no es null o vacío
                        if (!string.IsNullOrWhiteSpace(newColDB2) && !newColDB2.Equals(currentColDB2, StringComparison.OrdinalIgnoreCase))
                        {
                            toUpdateColumns.Add(newColDB2);
                        }
                    }
                }

                var jsonList = JsonConvert.SerializeObject(toUpdateColumns);
                //var outputValue = new SqlParameter
                //{
                //    ParameterName = "@@OutputResult",
                //    SqlDbType = SqlDbType.Bit,
                //    Direction = ParameterDirection.Output
                //};

                var outputResult = new SqlParameter
                {
                    ParameterName = "@result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                var outputDuplicateColumnName = new SqlParameter
                {
                    ParameterName = "@duplicateColumnName",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 255,
                    Direction = ParameterDirection.Output
                };


                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_VALIDATE_COLUMNS_PROJECT_UPDATE_TEMP]
                    @jsonInput = {jsonList},
                    @formId = {formId},
                    @result = {outputResult} OUTPUT,
                    @duplicateColumnName = {outputDuplicateColumnName} OUTPUT");

                var boolOutput = (bool)outputResult.Value;
                if (boolOutput)
                {
                    var updateList = toUpdate.Select(x =>
                    {
                        var normalizedColumnName = StringParser.NormalizeString(x.column_db_name);
                        var columnNameDB = normalizedColumnName;

                        var columnNameDB2 = x.column_db_name_2 != null ? StringParser.NormalizeString(x.column_db_name_2) : null;
                        var columnType2 = x.column_type_2 != null ? $"{x.column_type_2}" : null;

                        return new
                        {
                            columnId = x.id,
                            columnName = x.column_name,
                            columnType = x.column_type,
                            columnType2,
                            propsUi = x.props_ui,
                            state = x.hidden ? 0 : 1,
                            columnNameDB,
                            columnNameDB2
                        };
                    }).ToList();

                    var jsonParam = JsonConvert.SerializeObject(updateList);

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_UPDATE_COLUMNS @jsonInput={jsonParam}, @formId={formId};");

                }
                else
                {
                    return StatusCode(500, new ItemResp { status = 500, message = "NAME_COLUMN_ALREADY_EXISTS", data = outputDuplicateColumnName.Value });
                }
            }


            if (toInsert.Any())
            {
                Console.WriteLine(JsonConvert.SerializeObject(toInsert));
                var insertList = toInsert.Select(x =>
                {
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
                        columnType2,
                        propsUi = x.props_ui,
                        formId,
                        state = 1,
                        questionTypeId = x.question_type_id,
                        columnNameDB,
                        columnNameDB2
                    };
                }).ToList();

                var jsonParameter = JsonConvert.SerializeObject(insertList);

                var outputResult = new SqlParameter
                {
                    ParameterName = "@result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                var outputDuplicateColumnName = new SqlParameter
                {
                    ParameterName = "@duplicateColumnName",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 255,
                    Direction = ParameterDirection.Output
                };



                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_VALIDATE_COLUMNS_PROJECT_TEMP]
                    @jsonInput = {jsonParameter},
                    @formId = {formId},
                    @result = {outputResult} OUTPUT,
                    @duplicateColumnName = {outputDuplicateColumnName} OUTPUT");

                var boolOutput = (bool)outputResult.Value;
                if (boolOutput)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC SP_INSERT_COLUMNS @jsonInput={jsonParameter}, @formId={formId};");
                    //return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { questionDTO } });

                }
                else
                {
                    return StatusCode(500, new ItemResp { status = 500, message = "NAME_COLUMN_ALREADY_EXISTS", data = outputDuplicateColumnName.Value });
                }
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
