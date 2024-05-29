using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;

namespace ApiRestCuestionario.Controllers
{
    public class MultipleInsertOrganization
    {
        public int projectId { get; set; }
        public required List<Organizacion> organizations { get; set; }
    }

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
                Organizacion organizacionSave = JsonConvert.DeserializeObject<Organizacion>(value.ToString())!;

                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = organizacionSave.IdOrganizacion ?? (object)DBNull.Value
                };

                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_ORGANIZACION] 
                    @IdOrganizacion={idParameter} OUTPUT, 
                    @CodigoOrganizacion={organizacionSave.CodigoOrganizacion},
                    @NomOrganizacion={organizacionSave.NomOrganizacion},
                    @IdProyecto={organizacionSave.IdProyecto},
                    @IdLocalidad={organizacionSave.IdLocalidad},
                    @NivelRiesgoGeneral={organizacionSave.NivelRiesgoGeneral},
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
            catch (SqlException ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }

        }

        [HttpPost("MultipleInsertOrganization")]
        public async Task<ActionResult> SaveMultipleOrganization([FromBody] JsonElement value)
        {
            try
            {
                List<Organizacion> organizaciones = JsonConvert.DeserializeObject<List<Organizacion>>(value.ToString())!;

                var gruposPorProyecto = organizaciones.GroupBy(o => o.IdProyecto)
                                             .Select(group => new
                                             {
                                                 Proyecto = group.Key,
                                                 Codigos = String.Join(",", group.Select(g => g.CodigoOrganizacion))
                                             });

                foreach (var grupo in gruposPorProyecto)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_CHECK_ORGANIZATION]
                    @Codigos={grupo.Codigos},
                    @Proyecto={grupo.Proyecto};");
                }


                foreach (Organizacion organizacion in organizaciones)
                {
                    var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = organizacion.IdOrganizacion ?? (object)DBNull.Value
                    };

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_ORGANIZACION] 
                    @IdOrganizacion={idParameter} OUTPUT, 
                    @CodigoOrganizacion={organizacion.CodigoOrganizacion},
                    @NomOrganizacion={organizacion.NomOrganizacion},
                    @IdProyecto={organizacion.IdProyecto},
                    @IdLocalidad={organizacion.IdLocalidad},
                    @NivelRiesgoGeneral={organizacion.NivelRiesgoGeneral},
                    @Latitud={organizacion.Latitud},
                    @Longitud={organizacion.Longitud},
                    @Estado={organizacion.Estado},
                    @IdUsuario={organizacion.IdUsuario};");
                }

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = e.ToString(), data = e.ToString() });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }

        }

        //[HttpPost("EditOrganizacion")]
        //public ActionResult EditPerson([FromBody] JsonElement value)
        //{
        //    try
        //    {
        //        int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
        //        Organizacion organizacionSave = JsonConvert.DeserializeObject<Organizacion>(value.GetProperty("organizacion").ToString());
        //        Organizacion organizacionValidate = null;
        //        organizacionValidate = context.Organizacion.ToList().AsReadOnly().Where(c => c.ID_ORGANIZACION == organizacionSave.ID_ORGANIZACION).FirstOrDefault();
        //        if(organizacionValidate != null)
        //        {
        //            if (organizacionSave.idorganizacion != organizacionValidate.idorganizacion)
        //            {
        //                return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });
        //            }
        //        }
        //        List<Organizacion_localidad> organizacion_localidadupdate = JsonConvert.DeserializeObject<List<Organizacion_localidad>>(value.GetProperty("organizacionLocalidad").ToString());
        //        List < Organizacion_localidad > organizacion_localidadSave = new List<Organizacion_localidad>();
        //        foreach (Organizacion_localidad c in organizacion_localidadupdate)
        //        {
        //            if(c.idOrganizacionLocalidad == 0)
        //            {
        //                context.Organizacion_Localidad.Add(c);
        //            }
        //            else
        //            {
        //         a       context.Organizacion_Localidad.Update(c);
        //            }
        //        }

        //        context.Organizacion.Update(organizacionSave);
        //        context.SaveChanges();
        //        return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = organizacionSave });
        //    }
        //    catch (InvalidCastException e)
        //    {
        //        return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
        //    }

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

        [HttpGet]
        [Route("GetOrganizacionesByPersona")]
        public async Task<ActionResult> GetOrganizacionesByPersona(int IdProyecto, int IdPersona)
        {
            try
            {
                var List = await context.Persona_Organizacion.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_ORGANIZACION_POR_PERSONA] @IdProyecto={IdProyecto}, @IdPersona={IdPersona}").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = List });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

        [HttpGet]
        [Route("GetOrganizacionByLocalidad")]
        public async Task<ActionResult> GetOrganizacionByLocalidad(int IdProyecto, int IdLocalidad)
        {
            try
            {
                //int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                //object ListOrganizacion = context.Organizacion.ToList();
                var ListOrganizacion = await context.Organizacion.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_ORGANIZACION_POR_PROYECTO_LOCALIDAD] @IdProyecto={IdProyecto}, @IdLocalidad={IdLocalidad}").ToListAsync();
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
                List<Organizacion_localidad> organizacion_localidadSave = JsonConvert.DeserializeObject<List<Organizacion_localidad>>(value.GetProperty("organizacionLocalidad").ToString())!;
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
