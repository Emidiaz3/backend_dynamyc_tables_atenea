using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizacionController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public OrganizacionController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost("SaveOrganizacion")]
        public async Task<ActionResult> SavePerson([FromBody] JsonElement value)
        {
            try
            {
                Organizacion organizacionSave = JsonConvert.DeserializeObject<Organizacion>(value.ToString());
           
                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = organizacionSave.Id ?? (object)DBNull.Value
                };

                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_ORGANIZACION] 
                    @Id={idParameter} OUTPUT, 
                    @Id_Organizacion={organizacionSave.Id_Organizacion},
                    @NomOrganizacion={organizacionSave.NomOrganizacion},
                    @Proyecto={organizacionSave.Proyecto},
                    @Localidad={organizacionSave.Localidad},
                    @Nivel_Riesgo_General={organizacionSave.Nivel_Riesgo_General},
                    @Latitud={organizacionSave.Latitud},
                    @Longitud={organizacionSave.Longitud},
                    @Estado={organizacionSave.Estado},
                    @IdUsuario={organizacionSave.IdUsuario}
                ;");
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = idParameter });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = e.ToString(), data = e.ToString() });
            }

        }

        [HttpGet("GetOrganizacion")]
        public async Task<ActionResult> GetPerson(int IdProyecto)
        {
            try
            {
                var ListOrganizacion = await context.Organizacion.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_ORGANIZACION_POR_PROYECTO] @IdProyecto={IdProyecto}").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListOrganizacion });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpPost("GetOrganizacionlocalidadById")]
        public ActionResult GetOrganizaciónById([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                int organizacion_id = JsonConvert.DeserializeObject<int>(value.GetProperty("organizacion").GetProperty("organizacion_id").ToString());
                
                object listLocalidad = context.Organizacion_Localidad.Where(c => organizacion_id == c.idOrganizacion).ToList();
                
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = listLocalidad });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [HttpPost("DeleteOrganizacionlocalidad")]
        public ActionResult DeleteOrganizacionlocalidad([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                List<Organizacion_localidad> organizacion_localidadSave = JsonConvert.DeserializeObject<List<Organizacion_localidad>>(value.GetProperty("organizacionLocalidad").ToString());
                context.Organizacion_Localidad.RemoveRange(organizacion_localidadSave);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
    }
}
