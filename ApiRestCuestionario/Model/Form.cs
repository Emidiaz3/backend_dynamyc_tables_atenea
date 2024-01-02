using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Form
    {
        public int id { get; set; }
        public string form_name { get; set; }
        public string? form_db_name { get; set; }
        public string form_label { get; set; }
        public string form_abstract { get; set; }
        public string status { get; set; }
        public string link { get; set; }
        public string archive { get; set; }
        public int? IdProyecto { get; set; }
    }
}
