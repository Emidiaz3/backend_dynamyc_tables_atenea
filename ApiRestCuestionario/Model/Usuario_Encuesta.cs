using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Usuario_Encuesta
    {
        public int id { get; set; }
        public string nombreForm { get; set; }
        public int idForm { get; set; }
        public int idUsuario { get; set; }

        public string linkForm { get; set; }

        public int? idTipoEncuesta { get; set; }


    }
}
