using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text.Json;

public class dataJoinForm
{
    public required object userForm { get; set; }
    public object? formList { get; set; }

}
public class dataJoinAnswer
{
    public object? dataAnswer { get; set; }
    public required object dataQuestion { get; set; }

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
            this.connectionString = configuration.GetConnectionString("Database")!;
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

        [HttpPost] // Asegúrate de especificar el verbo HTTP adecuado
        [Route("GetUserFormsByProjectAux/{projectId}")]
        public ActionResult PostAuxByProject([FromBody] JsonElement value, int projectId)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("users").GetProperty("users_id").ToString());

                var userForm = context.Form
                    .Join(context.Usuario_Encuesta,
                        form => form.id,
                        usuarioEncuesta => usuarioEncuesta.form_id,
                        (form, usuarioEncuesta) => new { Form = form, UsuarioEncuesta = usuarioEncuesta })
                    .Where(x => x.UsuarioEncuesta.users_id == user_id
                                && x.Form.IdProyecto == projectId)
                    .ToList();

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new dataJoinForm { formList = null, userForm = userForm } });
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
                var usersFormJson = value.GetProperty("users_form").ToString();
                Users_Form user_form = JsonConvert.DeserializeObject<Users_Form>(usersFormJson);

                Users_Form? userForm = context.Users_Form.FirstOrDefault(uf => uf.users_id == user_form.users_id && uf.form_id == user_form.form_id);

                if (userForm == null)
                {
                    return NotFound();
                }

                userForm.state = "0";

                context.Users_Form.Update(userForm);
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
                object? answerList = null;

                answerList = context.Answers.Where(c => c.form_id == form_id).ToList().Where(c => (!c.Flg_proceso!.Equals("4") && !c.Flg_proceso.Equals("5"))).OrderBy(c => c.answer_date);


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
                object? answerList = null;

                answerList = context.Answers.Where(c => c.form_id == form_id).ToList().Where(c => (!c.Flg_proceso!.Equals("4") && !c.Flg_proceso.Equals("5"))).OrderBy(c => c.answer_date).Where(c => c.users_id == user_id);


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
            }
            catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });

            }


        }

        [HttpGet("GetAnswers")]
        public ActionResult GetAnswers([FromQuery][Required] int formId, [FromQuery][Required] int pageNumber = 1, [FromQuery][Required] int pageSize = 10)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query("sp_dynamic_report", new { formId, PageNumber = pageNumber, PageSize = pageSize }, commandType: CommandType.StoredProcedure).ToList();
                var columns = connection.Query("SP_OBTENER_COLUMNAS", new { formId }, commandType: CommandType.StoredProcedure).ToList();
                connection.Close();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { rows = rows ?? new List<dynamic>(), columns = columns ?? new List<dynamic>() } });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });
            }
        }

        [HttpGet("GetFilteredAnswers")]
        public ActionResult GetFilteredAnswers([FromQuery][Required] int formId, [FromQuery][Required] int pageNumber, [FromQuery][Required] int pageSize, [FromQuery] string searchQuery)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Query("sp_dynamic_report_search", new { formId, PageNumber = pageNumber, PageSize = pageSize, SearchQuery = searchQuery }, commandType: CommandType.StoredProcedure).ToList();
                var columns = connection.Query("SP_OBTENER_COLUMNAS", new { formId }, commandType: CommandType.StoredProcedure).ToList();
                connection.Close();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = new { rows = rows ?? new List<dynamic>(), columns = columns ?? new List<dynamic>() } });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });
            }
        }

        [HttpGet("GetUniqueColumnValues")]
        public ActionResult GetUniqueColumnValues([FromQuery][Required] int formId, [FromQuery][Required] string columnName)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var uniqueValues = connection.Query<string>("sp_get_unique_column_values", new { formId, columnName }, commandType: CommandType.StoredProcedure).ToList();
                connection.Close();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = uniqueValues ?? new List<string>() });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ItemResp { status = 200, message = e.ToString(), data = null });
            }
        }




      
        [HttpGet("GetFilteredDynamicReport")]
        public ActionResult GetFilteredDynamicReport([FromQuery][Required] int formId, [FromQuery][Required] int pageNumber, [FromQuery][Required] int pageSize, [FromQuery] string filters)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Verifica y convierte el formato de fecha en C# si es necesario
                var filterObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(filters);
                var updatedFilterObj = new Dictionary<string, string>();

                foreach (var filter in filterObj)
                {
                    DateTime parsedDate;
                    // Intenta convertir el valor en distintos formatos posibles
                    if (DateTime.TryParseExact(filter.Value, new[] { "MM/dd/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        updatedFilterObj[filter.Key] = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss"); // Formatea la fecha correctamente
                    }
                    else
                    {
                        updatedFilterObj[filter.Key] = filter.Value;
                    }
                }

                filters = JsonConvert.SerializeObject(updatedFilterObj);

                var rows = connection.Query("sp_dynamic_filtered_report", new { formId, PageNumber = pageNumber, PageSize = pageSize, Filters = filters }, commandType: CommandType.StoredProcedure).ToList();
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
