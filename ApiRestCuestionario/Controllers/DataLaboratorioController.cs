using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

class deleteObject{
    public string mes { get; set; }
    public int anio { get; set; }

}


namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataLaboratorioController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";

        public DataLaboratorioController(AppDbContext context)
        {
            this.context = context;
        }
        [Route("dataLaboratorio")]
        [HttpPost]
        public ActionResult dataLaboratorio([FromBody] JsonElement value)
        {
            try
            {


                List<Data_Laboratorio> listDatosLaboratorio = JsonConvert.DeserializeObject<List<Data_Laboratorio>>(value.GetProperty("datosLaboratorio").ToString());
                context.Data_Laboratorio.AddRange(listDatosLaboratorio);
                context.SaveChanges();

                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [Route("getDataLaboratorio")]
        [HttpGet]
        public ActionResult getDataLaboratorio()
        {
            try
            {


                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data =
                    context.Data_Laboratorio.GroupBy(c => new { anio=c.cod_periodo, c.mes }).Select(c =>   new { group=c.Key, cantidad = c.Count() ,empresa= "laboratorio" }).OrderBy(c=> c.group.anio).ThenBy(c=>c.group.mes)
                });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [Route("getDataLaboratorioByAnioMes")]
        [HttpPost]
        public ActionResult getDataLaboratorioByAnioMes([FromBody] JsonElement value)
        {
            try
            {

                deleteObject mesdata = JsonConvert.DeserializeObject<deleteObject>(value.GetProperty("mesdata").ToString());

                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = CONFIRM,
                    data =
                    context.Data_Laboratorio.ToList().Where(c=>c.cod_periodo== mesdata.anio).Where(c=>c.mes== mesdata.mes)
                });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [Route("deleteDataLaboratorio")]
        [HttpPost]
        public ActionResult deleteDataLaboratorio([FromBody] JsonElement value)
        {
            try
            {

                deleteObject mesdelete = JsonConvert.DeserializeObject<deleteObject>(value.GetProperty("mesdelete").ToString());
                
                context.Data_Laboratorio.RemoveRange(context.Data_Laboratorio.Where(x =>  x.mes == mesdelete.mes).Where(x=>x.cod_periodo == mesdelete.anio));
                context.SaveChanges();

                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = "Eliminado Exitosamente",
                    data =null
                });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
        [Route("executeProcedure")]
        [HttpGet]
        public async Task<ActionResult<ItemResponse>> executeProcedure()
        {
            try
            {
               await context.Database
               .ExecuteSqlInterpolatedAsync($@"Exec SP_EJECUCION_CRUCE_PLANILLAS");
                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = "Ejecutado Exitosamente",
                    data = null
                });
            }
            catch (InvalidCastException e)
            {
                return StatusCode(404, new ItemResp { status = 200, message = CONFIRM, data = e.ToString() });
            }

        }
    }
}
