using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    class Quest
    {
        public int number;
        public string column_type;
        public string column_name;
        public string column_db_name;
        public string props_ui;
    }

    public class ColumnInfo
    {
        public int? id { get; set; }
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
                object questions = context.column_types.Where(c => c.form_id == formId && c.props_ui != null);
                object aparence = context.Form_Aparence.FirstOrDefault(c => c.form_id == formId);
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new  {aparence,questions } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        //[HttpPost]
        //public ActionResult Post([FromBody] JsonElement value)
        //{
        //    List<Questions> questionsSave = JsonConvert.DeserializeObject<List<Questions>>(value.GetProperty("questions").ToString());
        //    Form_Aparence aparenceSave = JsonConvert.DeserializeObject<Form_Aparence>(value.GetProperty("aparence").ToString());

        //    int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
        //    foreach (Questions quest in questionsSave)
        //    {
        //        quest.form_id = form_id;
        //    }
        //    List<Questions> questionsSaveNotRepeat = questionsSave.Where(x => x.id == null).ToList();
        //    context.Questions.AddRange(questionsSaveNotRepeat);
        //    //Se Crea una lista con los elementos a hacer update
        //    List<Questions> questionsUpdate = questionsSave.Where(x => x.id != null).ToList();
        //    context.Questions.UpdateRange(questionsUpdate);
        //    context.Form_Aparence.Add(aparenceSave);
        //    if (aparenceSave.id != 0)
        //    {
        //        context.Form_Aparence.Update(aparenceSave);
        //    }

        //    //Se guarda los cambios
        //    context.SaveChanges();
        //    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = questionsSave });
        //}

        [HttpDelete]
        public ActionResult DeleteQuestions([FromBody] JsonElement value)
        {
            try
            {
                List<Questions> questions_ListId = JsonConvert.DeserializeObject<List<Questions>>(value.GetProperty("questions").GetProperty("questionsList").ToString());

                if (questions_ListId.Count() > 0)
                {
                    foreach (Questions quest in questions_ListId)
                    {
                        List<Answers> answer = context.Answers.Where(c => c.questions_id == quest.id).ToList();
                        if (questions_ListId.Count() > 0)
                        {
                            context.Answers.RemoveRange(answer);
                        }
                    }
                    context.Questions.RemoveRange(questions_ListId);
                }
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ""});
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        
        
        [HttpPost]
        public async Task<ActionResult> SaveQuestions([FromBody] JsonElement value)
        {
            Form_Aparence aparenceSave = JsonConvert.DeserializeObject<Form_Aparence>(value.GetProperty("aparence").ToString());
            var questions = JsonConvert.DeserializeObject<List<Quest>>(value.GetProperty("questions").ToString());
            int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
            var columns = context.column_types.Where(x => x.form_id == form_id).Select(x=>x.nombre_columna_db).ToList();
            Dictionary<string, int> itemsCounter = new Dictionary<string, int>();

            context.Form_Aparence.Add(aparenceSave);
            if (aparenceSave.id != 0)
            {
                context.Form_Aparence.Update(aparenceSave);
            }

            //Se guarda los cambios
            context.SaveChanges();
            foreach (var x in columns) {

                var items = x.Trim().Split("_");
                var isNumeric = int.TryParse(items.Last(), out int n);

                if (items.Length > 1 && isNumeric)
                {
                    var verificationString = string.Join("_", items.Take(items.Length - 1));
                    if (itemsCounter.ContainsKey(verificationString) && n > itemsCounter[verificationString])
                    {
                        itemsCounter[verificationString] = n;

                    } else
                    {
                        itemsCounter[x] = 1;
                    }

                } else
                {
                    itemsCounter[x] = 1;
                }
            }
            var columnNames = string.Join(",", questions.Select(x => x.column_name));
       
            var columnNamesDB = string.Join(",", questions.Select(x =>
            {
                string normalizedString = x.column_db_name.Normalize(NormalizationForm.FormD);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (char c in normalizedString)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                        stringBuilder.Append(c);
                }
                ;
                var item = stringBuilder.ToString().Trim().ToLower().Replace(" ", "_");
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
            var columnTypes = string.Join(",", questions.Select(x => x.column_type));
            var props_ui = JsonConvert.SerializeObject(questions.Select(x => x.props_ui));
           
            var storedProcedureName = "AddColumnsAndInsertData";
            
            var result = await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC {storedProcedureName} @columnNames={columnNames}, @columnNamesDB={columnNamesDB}, @columnTypes={columnTypes}, @props_ui = {props_ui}, @formId={form_id};");


            return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { columns, columnNames, columnNamesDB, columnTypes, props_ui, form_id }  });

        }

        [HttpGet("types")]
        public async Task<ActionResult> GetQuestionTypes()
        {
            var data = await context.question_types.ToListAsync();
          
            return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos con éxito", data=data });
        }

        //[HttpPost("types")]
        //public async Task<ActionResult> SaveQuestionTypes([FromBody] JsonElement value)
        //{
        //    var questions = JsonConvert.DeserializeObject<List<QuestionType>>(value.GetProperty("types").ToString());
        //    context.question_types.AddRange(questions);
        //    var response = await context.SaveChangesAsync();
        //    return StatusCode(200, new ItemResp { status = 200, message = "Datos obtenidos con éxito", data = new { response } });
        //}

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

        [HttpPost("UpdateColumns")]
        public async Task<ActionResult> UpdateColumns([FromBody] JsonElement value)
        {
            try
            {
                var questionsRoot = JsonConvert.DeserializeObject<List<ColumnInfo>>(value.GetProperty("questions").GetRawText());
                var questionsDelete = JsonConvert.DeserializeObject<List<ColumnInfo>>(value.GetProperty("questionsDelete").GetRawText());


                // Cambiar estado a 0, preguntas/columnas que se quitaron
                foreach (var questionD in questionsDelete)
                {
                    var res1 = await context.Database.ExecuteSqlInterpolatedAsync($"Exec dbo.UpdateStateById @id={questionD.id}, @newState={0}");
                }

                foreach (var question in questionsRoot)
                {
                    string propsUiJsonE = JToken.FromObject(question.props_ui).ToString(Formatting.None);

                    //if (question.id == null)
                    //{
                    //    // insertar nuevas preguntas/columnas

                    //}
                    if (question.id != null)
                    {
                        // modificar preguntas/columnas existentes
                        var result = await context.Database.ExecuteSqlInterpolatedAsync($"Exec [dbo].[SP_UPDATE_COLUMN] @idColumn={question.id}, @columnName={question.columnName}, @columnNameDB={question.columnDBName},@dataType={question.columnType},@propsUi={propsUiJsonE}");
                    }
                }

                return Ok(new { status = 200, message = "Verificación completada." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }
        }



    }
}
