using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Questions
    {
        public Questions(){}
        public int? id { get; set; }
        public string title { get; set; }
        public string sugerence { get; set; }
        public int? position { get; set; }
        public bool necessary_question { get; set; }
        public string text { get; set; }
        public bool necessary_recuent { get; set; }
        public int? recuent_min { get; set; }
        public int? recuent_max { get; set; }
        public string options { get; set; }
        public string options_selec { get; set; }
        public int? calification_num { get; set; }
        public int? calification { get; set; }
        public string calification_icon { get; set; }
        public DateTime? date_time { get; set; }
        public string date_time_type { get; set; }
        public DateTime? date_time_init_limitation { get; set; }
        public DateTime? date_time_finally_limitation { get; set; }
        public string type_aparence_date { get; set; }
        public double? latitud { get; set; }
        public double? longitud { get; set; }
        public string center { get; set; }
        public string typeDraw { get; set; }
        public string type_origin { get; set; }

        public int? zoom { get; set; }
        public string optionsMap { get; set; }
        public string map { get; set; }
        public string marker { get; set; }
        public string text_information { get; set; }
        public string type_recuent_image { get; set; }
        public int? recuent_especific { get; set; }
        public int? recuent_min_archive { get; set; }
        public int? recuent_max_archive { get; set; }
        public string name_archiveordocument { get; set; }
        public string ubication_archiveordocument { get; set; }
        public int? size_archive { get; set; }
        public string type_select { get; set; }
        public string options_document { get; set; }
        public string answer_document { get; set; }
        public string answer_archive { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int? form_id { get; set; }
        public string calculo { get; set; }

    }
}
