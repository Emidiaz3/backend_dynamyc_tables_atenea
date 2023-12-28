using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportacionCJPController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";

        public ImportacionCJPController(AppDbContext context)
        {
            this.context = context;
        }
        [Route("dataLaboratorio")]
        [HttpPost]
        public ActionResult dataLaboratorio([FromBody] JsonElement value)
        {
            try
            {
                //IMPORTACION DE DATOS DEL EXCEL

                List<ImportacionCJP> listDatosLaboratorio = JsonConvert.DeserializeObject<List<ImportacionCJP>>(value.GetProperty("datosLaboratorio").ToString());
                context.ImportacionCJP.AddRange(listDatosLaboratorio);
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

                //Lista General
                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = CONFIRM,
                    data =
                    context.ImportacionCJP.GroupBy(c => new { anio = c.AÑO, c.MES }).Select(c => new { group = c.Key, cantidad = c.Count(), empresa = "laboratorio" }).OrderBy(c => c.group.anio).ThenBy(c => c.group.MES)
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
                //Lista Detalle
                deleteObject mesdata = JsonConvert.DeserializeObject<deleteObject>(value.GetProperty("mesdata").ToString());

                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = CONFIRM,
                    data =
                    context.ImportacionCJP.ToList().Where(c => c.AÑO == mesdata.anio).Where(c => c.MES == mesdata.mes)
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

                context.ImportacionCJP.RemoveRange(context.ImportacionCJP.Where(x => x.MES == mesdelete.mes).Where(x => x.AÑO == mesdelete.anio));
                context.SaveChanges();

                return StatusCode(200, new ItemResp
                {
                    status = 200,
                    message = "Eliminado Exitosamente",
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
