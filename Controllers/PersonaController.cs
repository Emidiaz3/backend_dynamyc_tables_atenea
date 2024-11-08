﻿using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace angular.Server.Controllers
{
    public class MultipleInsertPersona
    {
        public required Persona person { get; set; }
        public required List<InsertPersonaOrganization> insertPersonaOrganizacionList { get; set; }
    }

    public class SavePersonDto
    {
        public required Persona person { get; set; }
        public required List<Persona_Organizacion> insertPersonaOrganizacionList { get; set; }
        public required List<Persona_Organizacion> deletePersonaOrganizacionList { get; set; }
    }


    public class InsertPersonaOrganization
    {
        public int IdOrganizacion { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public PersonaController(AppDbContext context)
        {
            this.context = context;
        }

        [Route("SavePerson")]
        [HttpPost]
        public async Task<ActionResult> SavePerson([FromBody] SavePersonDto dto)
        {
            try
            {
                Persona persona = dto.person;

                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = persona.IdPersona ?? (object)DBNull.Value
                };

                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_PERSONA] 
                    @IdPersona={idParameter} OUTPUT, 
                    @CodigoPersona={persona.CodigoPersona},
                    @NomPersona={persona.NomPersona},
                    @IdProyecto={persona.IdProyecto},
                    @IdLocalidad={persona.IdLocalidad},
                    @NivelRiesgoGeneral={persona.NivelRiesgoGeneral},
                    @Latitud={persona.Latitud},
                    @Longitud={persona.Longitud},
                    @TipoDocumento={persona.TipoDocumento},
                    @NumeroDocumento={persona.NumeroDocumento},
                    @Estado={persona.Estado},
                    @IdUsuario={persona.IdUsuario}
                ;");

                if (persona.IdPersona != null)
                {
                    List<Persona_Organizacion> removeList = dto.deletePersonaOrganizacionList;
                    foreach (Persona_Organizacion persona_Organizacion in removeList)
                    {
                        await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_DELETE_SPECIFIC_PERSONA_ORGANIZACION] 
                        @IdPersona={idParameter},
                        @IdOrganizacion={persona_Organizacion.IdOrganizacion},
                        @Proyecto={persona.IdProyecto};");
                    }
                }

                List<Persona_Organizacion> insertList = dto.insertPersonaOrganizacionList;

                foreach (Persona_Organizacion persona_Organizacion in insertList)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_PERSONA_ORGANIZACION] 
                    @IdPersona={idParameter},
                    @IdOrganizacion={persona_Organizacion.IdOrganizacion},
                    @Proyecto={persona.IdProyecto};");
                }

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }
            catch (SqlException ex)

            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }

        }

        [HttpPost("MultipleInsertPerson")]
        public async Task<ActionResult> MultipleInsertPerson([FromBody] List<MultipleInsertPersona> list)
        {
            try
            {

                var gruposPorProyecto = list.GroupBy(x => x.person.IdProyecto)
                                             .Select(group => new
                                             {
                                                 Proyecto = group.Key,
                                                 Codigos = string.Join(",", group.Select(g => g.person.CodigoPersona))
                                             });



                foreach (var grupo in gruposPorProyecto)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_CHECK_PERSONA]
                    @Codigos={grupo.Codigos},
                    @Proyecto={grupo.Proyecto};");
                }


                foreach (MultipleInsertPersona insertPersona in list)
                {
                    var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = insertPersona.person.IdPersona ?? (object)DBNull.Value
                    };

                    await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_PERSONA] 
                        @IdPersona={idParameter} OUTPUT, 
                        @CodigoPersona={insertPersona.person.CodigoPersona},
                        @NomPersona={insertPersona.person.NomPersona},
                        @IdProyecto={insertPersona.person.IdProyecto},
                        @IdLocalidad={insertPersona.person.IdLocalidad},
                        @NivelRiesgoGeneral={insertPersona.person.NivelRiesgoGeneral},
                        @Latitud={insertPersona.person.Latitud},
                        @Longitud={insertPersona.person.Longitud},
                        @TipoDocumento={insertPersona.person.TipoDocumento},
                        @NumeroDocumento={insertPersona.person.NumeroDocumento},
                        @Estado={insertPersona.person.Estado},
                        @IdUsuario={insertPersona.person.IdUsuario};");


                    foreach (InsertPersonaOrganization insert in insertPersona.insertPersonaOrganizacionList)
                    {
                        await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_PERSONA_ORGANIZACION] 
                            @IdPersona={idParameter},
                            @IdOrganizacion={insert.IdOrganizacion},
                            @Proyecto={insertPersona.person.IdProyecto};");
                    }
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

        //[Route("EditPerson")]
        //[HttpPost]
        //public ActionResult EditPerson([FromBody] JsonElement value)
        //{
        //    try
        //    {
        //        int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
        //        T_MAE_PERSONA personaSave = JsonConvert.DeserializeObject<T_MAE_PERSONA>(value.GetProperty("person").ToString());
        //        context.T_MAE_PERSONA.Update(personaSave);
        //        context.SaveChanges();
        //        return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = personaSave });
        //    }
        //    catch (InvalidCastException e)
        //    {
        //        return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
        //    }

        //}

        [HttpGet]
        [Route("GetPerson")]
        public async Task<ActionResult> GetPerson(int IdProyecto)

        {
            try
            {
                var ListPersona = await context.Persona.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_PERSONA_POR_PROYECTO] @IdProyecto={IdProyecto}").ToListAsync();

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListPersona });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }


        [HttpPost("GetPersonByTipoEncuesta")]
        public ActionResult GetPersonByTipoEncuesta([FromBody] JsonElement value)
        {
            try
            {
                int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                string[] tipoEncuesta = JsonConvert.DeserializeObject<string[]>(value.GetProperty("TipoEncuesta").ToString())!;

                List<T_MAE_PERSONA> ListPerson = context.T_MAE_PERSONA.ToList();
                List<T_MAE_PERSONA> newListPerson = [];

                foreach (T_MAE_PERSONA c in ListPerson)
                {
                    if (JsonConvert.DeserializeObject<string[]>(c.TipoEncuesta!)!.Intersect(tipoEncuesta).Any())
                    {
                        newListPerson.Add(c);
                    }
                }
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = newListPerson });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }

    }
}
