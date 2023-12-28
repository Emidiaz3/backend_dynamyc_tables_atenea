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
        [Key]
        public int? idorganizacion { get; set; }
        public string ID_ORGANIZACION { get; set; }
        public string NOMBRE { get; set; }
        public int? Localidad_ID { get; set; }
        public string ID_LOCALIDAD { get; set; }
        public double? longitudOrg { get; set; }
        public double? latitudOrg { get; set; }
        public DateTime? fecharegistro { get; set; }
        public bool flg_estado { get; set; }
        public int? flg_proceso { get; set; }
        public int? idusuario { get; set; }

    }
}
