using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Usuario_Encuesta
    {
        public int? id { get; set; }
        public string? form_name { get; set; }
        public int? form_id { get; set; }
        public int? users_id { get; set; }
        public string? link { get; set; }
        public int? idTipoEncuesta { get; set; }
    }
}
