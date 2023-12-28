using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Localidad
    {
        [Key]
        public int? idlocalidad { get; set; }
        public string ID_LOCALIDAD { get; set; }
        public int? idUnidadMinera { get; set; }
        public string localidad { get; set; }
        public double? latitud { get; set; }
        public double? longitud { get; set; }
        public DateTime? fecharegistro { get; set; }
        public bool flg_estado { get; set; }
        public int? flg_proceso { get; set; }
        public int? idusuario { get; set; }


    }
}
