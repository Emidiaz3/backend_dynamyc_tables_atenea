using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    public class LocalidadInfo
    {
        public int? id { get; set; }

        public string columnName { get; set; }
        public string columnDBName { get; set; }
        public string columnType { get; set; }
        public JObject props_ui { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LocalidadController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        public LocalidadController(AppDbContext context)
        {
            this.context = context;
        }
        // POST api/<PersonaController>
        [Route("SaveLocalidad")]
        [HttpPost]
        public async Task<ActionResult> SaveLocalidad([FromBody] JsonElement value)
        {
            try
            {
                //int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                Localidad localidadSave = JsonConvert.DeserializeObject<Localidad>(value.ToString());

                //Localidad localidadValidate = null;
                //localidadValidate = context.Localidad.ToList().Where(c => c.ID_LOCALIDAD == localidadSave.ID_LOCALIDAD).FirstOrDefault();

                //if (localidadValidate != null)
                //{
                //    return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });
                //}


                //context.Localidad.Add(localidadSave);
                //context.SaveChanges();

                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = localidadSave.Id ?? (object)DBNull.Value
                };

                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_LOCALIDAD] 
                    @Id={idParameter} OUTPUT, 
                    @Id_Localidad={localidadSave.Id_Localidad},
                    @NomLocalidad={localidadSave.NomLocalidad},
                    @Proyecto={localidadSave.Proyecto},
                    @Departamento={null},
                    @Provincia={null},
                    @Distrito={null},
                    @Latitud={localidadSave.Latitud},
                    @Longitud={localidadSave.Longitud},
                    @Estado={localidadSave.Estado},
                    @IdUsuario={localidadSave.IdUsuario}
                ;");



                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = idParameter });
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
        //[Route("EditLocalidad")]
        //[HttpPost]
        //public ActionResult EditPerson([FromBody] JsonElement value)
        //{
        //    try
        //    {
        //        int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
        //        Localidad localidadSave = JsonConvert.DeserializeObject<Localidad>(value.GetProperty("localidad").ToString());

        //        Localidad localidadValidate = null;
        //        localidadValidate = context.Localidad.ToList().AsReadOnly().Where(c => c.ID_LOCALIDAD == localidadSave.ID_LOCALIDAD).FirstOrDefault();
        //        if (localidadValidate != null)
        //        {
        //            if (localidadSave.idlocalidad != localidadValidate.idlocalidad)
        //            {
        //                return StatusCode(200, new ItemResp { status = 400, message = "El código ingresado ya se encuentra en uso", data = null });

        //            }
        //        }

        //        context.Localidad.Update(localidadSave);
        //        context.SaveChanges();
        //        return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = localidadSave });
        //    }
        //    catch (InvalidCastException e)
        //    {
        //        return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
        //    }

        //}

        [HttpGet]
        [Route("GetLocalidad")]
        public async Task<ActionResult>GetLocalidadesByProyecto(int IdProyecto)
        {
            try
            {
                //int user_id = JsonConvert.DeserializeObject<int>(value.GetProperty("user").GetProperty("user_id").ToString());
                //object ListLocalidad = context.Localidad.ToList();
                var ListLocalidad = await context.Localidad.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_LOCALIDADES_POR_PROYECTO] @IdProyecto={IdProyecto}").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListLocalidad });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
    }
}
