using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.Json;

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
    [ApiController]
    [Route("api/[controller]")]
    public class Users_FormController : ControllerBase
    {
        private readonly AppDbContext context;
        readonly string CONFIRM = "Se creo con exito";
        readonly string connectionString;
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
                var userForm = context.Form
                    .Join(context.Users_Form,
                        form => form.id,
                        usersForm => usersForm.form_id,
                        (form, usersForm) => new { Form = form, UsersForm = usersForm })
                    .Where(x => x.UsersForm.users_id == user_id && x.UsersForm.state == "1" && x.Form.IdProyecto != null)
                    .ToList();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinForm { formList = null, userForm = userForm } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost] // Asegúrate de especificar el verbo HTTP adecuado
        [Route("GetUserFormsByProject/{projectId}")]
        public ActionResult PostByProject([FromBody] JsonElement value, int projectId)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("users").GetProperty("users_id").ToString());

                var userForm = context.Form
                    .Join(context.Users_Form,
                        form => form.id,
                        usersForm => usersForm.form_id,
                        (form, usersForm) => new { Form = form, UsersForm = usersForm })
                    .Where(x => x.UsersForm.users_id == user_id
                                && x.UsersForm.state == "1"
                                && x.Form.IdProyecto == projectId) // Filtra por el projectId
                    .ToList();

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinForm { formList = null, userForm = userForm } });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }


        [HttpPost("GetAnswerCountNum")]
        public ActionResult GetAnswerCountNum([FromBody] JsonElement value)
        {

            try
            {
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

        [HttpPost("EditUsersFormStatus")]
        public ActionResult EditUsersFormStatus([FromBody] JsonElement value)
        {
            try
            {
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
        [HttpPost("GetResultAnswer")]
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

        [HttpPost("GetResultAnswerEncuestado")]
        public ActionResult GetResultAnswerEncuestado([FromBody] JsonElement value)
        {
            try
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("user_id").ToString());

                object questionResult = context.Questions.Where(c => c.form_id == form_id).ToList().OrderBy(c => c.position);
                object answerList = null;

                answerList = context.Answers.Where(c => c.form_id == form_id).ToList().Where(c => (!c.Flg_proceso.Equals("4") && !c.Flg_proceso.Equals("5"))).OrderBy(c => c.answer_date).Where(c => c.users_id == user_id);


                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinAnswer { dataAnswer = answerList, dataQuestion = questionResult } });
            }
            catch (InvalidCastException e)
            {
                int form_id = JsonConvert.DeserializeObject<int>(value.GetProperty("form").GetProperty("form_id").ToString());

                object questionResult2 = context.Questions.Where(c => c.form_id == form_id).ToList().OrderBy(c => c.position);

                return StatusCode(200, new ItemResp { status = 200, message = e.ToString(), data = new dataJoinAnswer { dataAnswer = null, dataQuestion = questionResult2 } });

            }

        }

        [HttpGet("GetAnswer")]
        public ActionResult GetAnswer([FromQuery][Required] int formId, [FromQuery][Required] int answerId)
        {
          try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query("SP_OBTENER_RESPUESTA", new { formId, answerId }, commandType: CommandType.StoredProcedure).ToList().FirstOrDefault();
                connection.Close();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = rows });
            } catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });

            }


        }

        [HttpGet("GetAnswers")]
        public ActionResult GetAnswers([FromQuery][Required] int formId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query("sp_dynamic_report", new { formId }, commandType: CommandType.StoredProcedure).ToList();
                var columns = connection.Query("SP_OBTENER_COLUMNAS", new { formId }, commandType: CommandType.StoredProcedure).ToList();
                connection.Close();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { rows = rows ?? new List<dynamic>(), columns = columns ?? new List<dynamic>() } });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });
            }
        }


    }
}
