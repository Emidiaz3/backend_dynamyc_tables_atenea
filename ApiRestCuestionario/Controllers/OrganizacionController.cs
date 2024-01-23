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
    public class MultipleInsertOrganization
    {
        public int projectId { get; set; }
        public List<Organizacion> organizations { get; set; }
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
                List<Organizacion> organizaciones = JsonConvert.DeserializeObject<List<Organizacion>>(value.ToString());

                var gruposPorProyecto = organizaciones.GroupBy(o => o.Proyecto)
                                             .Select(group => new
                                             {
                                                 Proyecto = group.Key,
                                                 Codigos = String.Join(",", group.Select(g => g.Id_Organizacion))
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
                        Value = organizacion.Id ?? (object)DBNull.Value
                    };

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_ORGANIZACION] 
                    @Id={idParameter} OUTPUT, 
                    @Id_Organizacion={organizacion.Id_Organizacion},
                    @NomOrganizacion={organizacion.NomOrganizacion},
                    @Proyecto={organizacion.Proyecto},
                    @Localidad={organizacion.Localidad},
                    @Nivel_Riesgo_General={organizacion.Nivel_Riesgo_General},
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
