﻿using ApiRestCuestionario.Context;
using ApiRestCuestionario.Model;
using ApiRestCuestionario.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteFinalController : ControllerBase
    {
        private readonly AppDbContext context;
        string CONFIRM = "Se creo con exito";
        //private readonly IConfiguration config;
        public ReporteFinalController(AppDbContext context)// IConfiguration _config
        {
            this.context = context;
            //this.config = _config;
        }
        [HttpGet]
        [Route("GetReporteFinal")]
        public ActionResult GetReporteFinal()
        {
            var response = new ItemResponse();

            try
            {

                var ReporteFinaldata = context.ReporteFinalL.FromSqlInterpolated($"Exec SP_REPORTE_FINAL ").AsAsyncEnumerable(); 
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ReporteFinaldata });
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }
        [HttpPost]
        [Route("GetReportByAnioMes")]
        public ActionResult GetReportByAnioMes([FromBody] JsonElement value)
        {
            var response = new ItemResponse();

            try
            {
                ReporteFinal listDatosLaboratorio = JsonConvert.DeserializeObject<ReporteFinal>(value.GetProperty("dataAnioMes").ToString());

                var ReporteFinaldata = context.ReporteFinal.FromSqlInterpolated($"Exec SP_REPORTE_FINAL_SELECT_BY_MES_ANIO @anio={listDatosLaboratorio.Año} ,@mes ={ listDatosLaboratorio.Numes}").AsAsyncEnumerable(); ;
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = ReporteFinaldata });
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }
        [HttpPost]
        [Route("DeleteReportByAnioMes")]
        public ActionResult DeleteReportByAnioMes([FromBody] JsonElement value)
        {
            var response = new ItemResponse();

            try
            {
                ReporteFinal listDatosLaboratorio = JsonConvert.DeserializeObject<ReporteFinal>(value.GetProperty("dataAnioMes").ToString());
                List<ReporteFinalDetail> lisdelete = context.ReporteFinal.Where(c => c.Numes == listDatosLaboratorio.Numes).Where(c => c.Año == listDatosLaboratorio.Año).ToList();

                context.ReporteFinal.RemoveRange(lisdelete);
                context.SaveChanges();
                return StatusCode(200, new ItemResp { status = 200, message = CONFIRM, data = null });
            }
            catch (SqlException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append((errorMessages.Length != 0 ? "\n" : "") + ex.Errors[i].Message);
                }

                response.status = 0;
                response.message = errorMessages.ToString();
                return Ok(response); ;
            }
        }
    }
}
