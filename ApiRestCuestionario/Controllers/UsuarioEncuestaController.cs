﻿using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
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
                Usuario_Encuesta UsuarioEncuestaSave = JsonConvert.DeserializeObject<Usuario_Encuesta>(form.GetProperty("usuario_encuesta").ToString());
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
        public ActionResult GetUsuarioEncuestaById([FromBody] JsonElement form)
        {
            try
            {
                int idUsuario = JsonConvert.DeserializeObject<int>(form.GetProperty("idUsuario").ToString());
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = context.Usuario_Encuesta.ToList().Where(c => c.users_id == idUsuario)});
            }
            catch (InvalidCastException e)
            {

                return BadRequest(e.ToString());
            }
        }

        [HttpPost("DeleteUsuarioEncuestaByObject")]
        public ActionResult DeleteUsuarioEncuestaById([FromBody] JsonElement form)
        {
            try
            {
                Usuario_Encuesta usuarioEncuesta = JsonConvert.DeserializeObject<Usuario_Encuesta>(form.GetProperty("DeleteUsuarioEncuestaByObject").ToString());
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
