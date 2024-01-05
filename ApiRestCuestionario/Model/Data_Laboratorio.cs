using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
	

	public class Data_Laboratorio
	{
		public int id { get; set; }
		public string periodo { get; set; }
		public string sede { get; set; }
		public double cod_periodo { get; set; }
		public string mes { get; set; }
		public string Fec_Atencion { get; set; }
		public double ide_atencion { get; set; }
		public double num_atencion { get; set; }
		public string txt_empresa_usu { get; set; }
		public string txt_paciente { get; set; }
		public string tip_atencion { get; set; }
		public double Cod_Prestacion { get; set; }
		public string Prestacion { get; set; }
		public string est_situacion_actual { get; set; }
		public double Cantidad { get; set; }
		public double vventa { get; set; }
		public double igv { get; set; }
		public double total { get; set; }
		public double pct_dscto { get; set; }
		public double imp_copago_vv { get; set; }
		public double oa { get; set; }
		public double Linea { get; set; }
		public double codigo_CJP { get; set; }
		public string txt_emp_oa { get; set; }
		public double ide_orden_pres { get; set; }
		public string ide_externo1 { get; set; }
		public double ide_externo2 { get; set; }

	}
}
