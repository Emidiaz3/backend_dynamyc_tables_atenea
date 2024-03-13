using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class TipoEncuesta
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int flg_estado { get; set; }
        public int idUsuario { get; set; }

        
    }
}
