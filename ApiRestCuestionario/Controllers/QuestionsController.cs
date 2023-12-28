using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestCuestionario.Controllers
{
    public class dataJoin
    {
        public dataJoin(){}
        public object aparence { get; set; }
        public object questions { get; set; }
        
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
        // GET: api/<QuestionsController>
        [HttpGet]
        public IEnumerable<Questions> Get()
        {
            return context.Questions.ToList();
        }
        


        // GET api/<QuestionsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        [Route("GetQuestions")]
        public ActionResult GetQuestions([FromBody] JsonElement value)
        {
            try
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                object questionsList = context.Questions.Where(c => c.form_id == form_id).OrderBy(c=>c.position).ToList();
                object form_aparence = context.Form_Aparence.Where(c => c.form_id == form_id).ToList();
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoin {aparence= form_aparence,questions= questionsList } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        
        [HttpPost]
        [Route("DeleteQuestions")]
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
        
        // POST api/<QuestionsController>
        [HttpPost]
        public ActionResult Post([FromBody] JsonElement value)
        {
            List<Questions> questionsSave = JsonConvert.DeserializeObject<List<Questions>>(value.GetProperty("questions").ToString());
            Form_Aparence aparenceSave = JsonConvert.DeserializeObject<Form_Aparence>(value.GetProperty("aparence").ToString());

            int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
            foreach (Questions quest in questionsSave)
            {
                quest.form_id = form_id;
            }
            List<Questions> questionsSaveNotRepeat = questionsSave.Where(x => x.id == null).ToList();
            context.Questions.AddRange(questionsSaveNotRepeat);
            //Se Crea una lista con los elementos a hacer update
            List<Questions> questionsUpdate= questionsSave.Where(x=>x.id != null).ToList();
            context.Questions.UpdateRange(questionsUpdate);
            context.Form_Aparence.Add(aparenceSave);
            if(aparenceSave.id != 0)
            {
                context.Form_Aparence.Update(aparenceSave);
            }

            //Se guarda los cambios
            context.SaveChanges();
            return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = questionsSave });
        }
        [HttpPost("test")]
        public ActionResult TestFormCreation([FromBody] JsonElement value)
        {
            List<Questions> questionsSave = JsonConvert.DeserializeObject<List<Questions>>(value.GetProperty("questions").ToString());
            int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
            var cuestions = questionsSave.Select(x =>
            {

                var parsedItem = x.title.Trim().ToLower().Replace(" ","_");
                
                return parsedItem;
            }).ToList();
            var elementTypes = string.Join(", ", questionsSave.Select(x => "NVARCHAR(MAX)"));
            Dictionary<string, int> contadorDeElementos = new Dictionary<string, int>();

            for (int i = 0; i < cuestions.Count; i++)
            {
                string pregunta = cuestions[i];

                if (contadorDeElementos.ContainsKey(pregunta))
                {
                    int contador = contadorDeElementos[pregunta];
                    contador++;
                    contadorDeElementos[pregunta] = contador;
                    cuestions[i] = $"{pregunta}_{contador}";
                }
                else
                {
                    contadorDeElementos.Add(pregunta, 1); 
                }
            }
            var storedProcedureName = "AddColumnsAndInsertData";
            var param1 = new SqlParameter("@columnNames", string.Join(", ", cuestions));
            var param2 = new SqlParameter("@columnTypes", elementTypes);
            var param3 = new SqlParameter("@formId", form_id);

            var result = context.Database.ExecuteSqlRaw($"EXEC {storedProcedureName} @columnNames, @columnTypes, @formId", param1, param2, param3);


            return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { form_id, cuestions , elementTypes }  });

        }

        
    }
}
