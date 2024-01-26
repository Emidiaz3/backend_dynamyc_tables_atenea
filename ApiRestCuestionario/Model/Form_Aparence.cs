using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Form_Aparence
    {
        public Form_Aparence() { }
        public int id { get; set; }
        public bool title_status { get; set; }
        public bool description_status { get; set; }
        public bool footer_status { get; set; }
        public string footer_text { get; set; }
        public string footer_url { get; set; }
        public string title { get; set; }
        public string title_opcion { get; set; }
        public string title_style { get; set; }
        public string description { get; set; }
        public string send { get; set; }
        public int form_id { get; set; }


    }
}
