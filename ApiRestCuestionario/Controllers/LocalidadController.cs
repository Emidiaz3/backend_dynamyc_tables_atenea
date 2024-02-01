using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
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

        [HttpGet("GetLocalidad")]
        public async Task<ActionResult> GetLocalidadesByProyecto(int IdProyecto)
        {
            try
            {
                var ListLocalidad = await context.Localidad.FromSqlInterpolated($"EXEC [dbo].[SP_LISTAR_LOCALIDADES_POR_PROYECTO] @IdProyecto={IdProyecto}").ToListAsync();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ListLocalidad });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }
        }

        [HttpPost("SaveLocalidad")]
        public async Task<ActionResult> SaveLocalidad([FromBody] JsonElement value)
        {
            try
            {
                Localidad localidadSave = JsonConvert.DeserializeObject<Localidad>(value.ToString());

                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = localidadSave.IdLocalidad ?? (object)DBNull.Value
                };

                await context.Database.ExecuteSqlInterpolatedAsync($@"EXEC [dbo].[SP_GUARDAR_LOCALIDAD] 
                    @IdLocalidad={idParameter} OUTPUT, 
                    @CodigoLocalidad={localidadSave.CodigoLocalidad},
                    @NomLocalidad={localidadSave.NomLocalidad},
                    @IdProyecto={localidadSave.IdProyecto},
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
      

     
    }
}
