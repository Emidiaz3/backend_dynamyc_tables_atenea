using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
public class dataJoinForm
{
    public dataJoinForm() { }
    public object userForm { get; set; }
    public object formList { get; set; }

}
public class dataJoinAnswer
{
    public dataJoinAnswer() { }
    public object dataAnswer { get; set; }
    public object dataQuestion { get; set; }

}
namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Users_FormController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        String connectionString;
        public Users_FormController(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.connectionString = configuration.GetConnectionString("Database");
        }
       
        [HttpPost]
        public ActionResult Post([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("users").GetProperty("users_id").ToString());
                //object formList = context.Users_Form.Where(c => c.users_id == user_id).ToList();
                object userForm = context.Form.Join(context.Users_Form, c => c.id, cm => cm.form_id, (c, cm) => new { form = c, userForm = cm }).Where(x=>x.userForm.users_id== user_id).ToList().Where(x=> x.userForm.state.Equals("1"));
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinForm { formList = null, userForm = userForm} });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }

        }

        [HttpPost]
        [Route("GetAnswerCountNum")]
        public ActionResult GetAnswerCountNum([FromBody] JsonElement value)
        {
            
            try
            {
                //Obtiene el numero de encuestas respondidas 
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                object form_aparence = context.Answers.Where(c => c.form_id == form_id).ToList();
                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = CONFIRM,
                    data = form_aparence
                });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("EditUsersFormStatus")]
        public ActionResult EditUsersFormStatus([FromBody] JsonElement value)
        {

            try
            {
                //Obtiene el numero de encuestas respondidas 
                Users_Form user_form = JsonConvert.DeserializeObject<Users_Form>(value.GetProperty("users_form").ToString());
                context.Users_Form.Update(user_form);
                context.SaveChanges();
                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = CONFIRM,
                    data = null
                });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost]
        [Route("GetResultAnswer")]
        public ActionResult GetResultAnswer([FromBody] JsonElement value)
        {
            try
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                object questionResult = context.Questions.Where(c => c.form_id == form_id).ToList().OrderBy(c => c.position);
                object answerList = null;
               
                answerList = context.Answers.Where(c => c.form_id == form_id).ToList().Where(c => (!c.Flg_proceso.Equals("4") && !c.Flg_proceso.Equals("5"))).OrderBy(c => c.answer_date);


                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinAnswer { dataAnswer = answerList, dataQuestion = questionResult } });
            }
            catch (InvalidCastException e)
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());

                object questionResult2 = context.Questions.Where(c => c.form_id == form_id).ToList().OrderBy(c => c.position);

                return StatusCode(200, new ItemResp { status = 200, message = e.ToString(), data = new dataJoinAnswer { dataAnswer = null, dataQuestion = questionResult2 } });

            }

        }
        [HttpPost]
        [Route("GetResultAnswerEncuestado")]
        public ActionResult GetResultAnswerEncuestado([FromBody] JsonElement value)
        {
            try
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("user_id").ToString());

                object questionResult = context.Questions.Where(c => c.form_id == form_id).ToList().OrderBy(c => c.position);
                object answerList = null;

                answerList = context.Answers.Where(c => c.form_id == form_id).ToList().Where(c => (!c.Flg_proceso.Equals("4") && !c.Flg_proceso.Equals("5"))).OrderBy(c => c.answer_date).Where(c=>c.users_id == user_id);


                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinAnswer { dataAnswer = answerList, dataQuestion = questionResult } });
            }
            catch (InvalidCastException e)
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());

                object questionResult2 = context.Questions.Where(c => c.form_id == form_id).ToList().OrderBy(c => c.position);

                return StatusCode(200, new ItemResp { status = 200, message = e.ToString(), data = new dataJoinAnswer { dataAnswer = null, dataQuestion = questionResult2 } });

            }

        }

        [HttpGet("GetAnswers")]
        public async Task<ActionResult> GetAnswers([FromQuery][Required] int formId)
        {
            Console.WriteLine(connectionString);
           try
            {
                using (var connection = new  SqlConnection(connectionString))
                {
                    connection.Open();
                    var data = connection.Query("sp_dynamic_report", new {formId}, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new {  data } });

                }
             }
            catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });

            }
        }

       
    }
}
