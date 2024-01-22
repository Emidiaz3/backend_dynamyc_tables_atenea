using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Organizacion
    {
        public Organizacion() { }
        //[Key]
        //public int? idorganizacion { get; set; }
        //public string ID_ORGANIZACION { get; set; }
        //public string NOMBRE { get; set; }
        //public int? Localidad_ID { get; set; }
        //public string ID_LOCALIDAD { get; set; }
        //public double? longitudOrg { get; set; }
        //public double? latitudOrg { get; set; }
        //public DateTime? fecharegistro { get; set; }
        //public bool flg_estado { get; set; }
        //public int? flg_proceso { get; set; }
        //public int? idusuario { get; set; }

        [Key]
        public int? Id { get; set; }
        public string Id_Organizacion { get; set; }
        public string? NomOrganizacion { get; set; }
        public int? Proyecto { get; set; }
        public int? Localidad { get; set; }
        public int? Nivel_Riesgo_General { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuario { get; set; }
        public int? Id_Ref { get; set; }

    }
}
