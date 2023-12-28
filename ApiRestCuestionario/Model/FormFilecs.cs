using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class FormFilecs
    {
        [NotMapped]
        public List<IFormFile> file { get; set; }
        [NotMapped]
        public string questionsId { get; set; }
        [NotMapped]
        public string formId { get; set; }
        [NotMapped]
        public string userId { get; set; }
        [NotMapped]
        public string hashUnic { get; set; }
        [NotMapped]
        public string Flg_proceso { get; set; }

    }
}
