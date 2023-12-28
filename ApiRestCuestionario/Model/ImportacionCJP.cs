using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class ImportacionCJP
    {
     public int ID { get; set; }
    public string MES { get; set; }
    public string EXPEDIENTE { get; set; }
    public string ORDEN_OA { get; set; }
    public string COD_SEGUS { get; set; }
    public string IdOrdenAtencion { get; set; }
    public string Secuencial { get; set; }
    public string IdProceso { get; set; }
    public string LineaOrdenAtencion { get; set; }
    public string CONSUMO { get; set; }
    public string FECHA { get; set; }
    public string EMPRESA { get; set; }
    public string MEDICO { get; set; }
    public string PACIENTE { get; set; }
    public string DESCRIPCION { get; set; }
    public double PORCOASEGURO { get; set; }
    public double COASEGURO { get; set; }
    public double BRUTO { get; set; }
    public double MARGEN { get; set; }
    public double IMP_SUB_TOTAL { get; set; }
    public double IGV { get; set; }
    public double IMP_NETO { get; set; }
    public string ASEGURADORA { get; set; }
    public string DES_ESPECIALIDAD_FACT_PROD { get; set; }
    public string DES_TIPO_PRESTACION_PROD { get; set; }
    public string DESMECPAGO { get; set; }
    public string HISTORIA { get; set; }
    public string TIPO_PROCESO { get; set; }
    public double AÑO { get; set; }
    public string PRESTACION { get; set; }
    public double MONTO_MARGEN { get; set; }
    public string FEC_INI_PRODUCCION { get; set; }
    public string FEC_FIN_PRODUCCION { get; set; }
    public string COMPROB { get; set; }
    public string FEC_ACCION { get; set; }
    public string FEC_MODIFICACION { get; set; }
}
}
