using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace ApiRestCuestionario.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioEncuestaController : ControllerBase
    {

        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";

        public UsuarioEncuestaController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost("SaveUsuarioEncuesta")]
        public ActionResult SaveUsuarioEncuesta([FromBody] JsonElement form)
        {
            try
            {
                Usuario_Encuesta UsuarioEncuestaSave = JsonConvert.DeserializeObject<Usuario_Encuesta>(form.GetProperty("usuario_encuesta").ToString())!;
                context.Usuario_Encuesta.AddRange(UsuarioEncuestaSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {

                return BadRequest(e.ToString());
            }
        }

        [HttpPost("GetUsuarioEncuestaByIdUsuario")]
        public async  Task<ActionResult> GetUsuarioEncuestaById([FromBody] JsonElement form)
        {
            try
            {


                int idUsuario = JsonConvert.DeserializeObject<int>(form.GetProperty("idUsuario").ToString());
                Console.WriteLine(idUsuario);
                var list = await context.UsuarioEncuesta.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_ENCUESTAS_POR_USUARIO] @usersId={idUsuario}").ToListAsync();

                // Filtrar directamente en la consulta a la base de datos.
                //var resultado = context.Usuario_Encuesta
                //    .Where(c => c.users_id == idUsuario)
                //    .ToList(); // Convertir a lista después de aplicar el filtro.

                return StatusCode(200, new ItemResp { status = 200, message = "Confirm", data = list });
            }
            catch (Exception e) // Captura excepciones más generales para simplificar.
            {
                // Considera loguear la excepción e para análisis.
                return BadRequest(e.Message); // Devuelve solo el mensaje de error para evitar exponer detalles innecesarios.
            }
        }


        [HttpPost("DeleteUsuarioEncuestaByObject")]
        public ActionResult DeleteUsuarioEncuestaById([FromBody] JsonElement form)
        {
            try
            {
                Usuario_Encuesta usuarioEncuesta = JsonConvert.DeserializeObject<Usuario_Encuesta>(form.GetProperty("DeleteUsuarioEncuestaByObject").ToString())!;
                context.Usuario_Encuesta.Remove(usuarioEncuesta);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null});
            }
            catch (InvalidCastException e)
            {

                return BadRequest(e.ToString());
            }
        }

        [HttpPost("GetTypeEncuestaByUserId")]
        public ActionResult GetTypeEncuestaByUserId([FromBody] JsonElement form)
        {
            try
            {
                int idUsuario = JsonConvert.DeserializeObject<int>(form.GetProperty("idUsuario").ToString());
                var listIdTipoEncuesta = context.Usuario_Encuesta.Where(c => c.users_id == idUsuario).Select(m => m.idTipoEncuesta).Distinct();
                if (listIdTipoEncuesta.Count() ==0)
                {
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = listIdTipoEncuesta });
                }
                else
                {
                    var listTipoEncuesta = context.TipoEncuesta.Where(r => listIdTipoEncuesta.Contains(r.id));
                    return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = listTipoEncuesta });
                }

            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }
        [HttpPost("GetUsuarioEncuestaByIdUsuarioByTypeEncuesta")]
        public ActionResult GetUsuarioEncuestaByIdUsuarioByTypeEncuesta([FromBody] JsonElement form)
        {
            try
            {
                int idUsuario = JsonConvert.DeserializeObject<int>(form.GetProperty("idUsuario").ToString());
                int idTipoEncuesta = JsonConvert.DeserializeObject<int>(form.GetProperty("idTipoEncuesta").ToString());
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = context.Usuario_Encuesta.ToList().Where(c => c.users_id == idUsuario).Where(c=>c.idTipoEncuesta== idTipoEncuesta) });
            }
            catch (InvalidCastException e)
            {
                return BadRequest(e.ToString());
            }
        }

    }
}
