
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ApiRestCuestionario.Dto
{
    public class SaveFormDocumentDto
    {
        public int formId { get; set; }
        public int questionsId { get; set; }
        public List<IFormFile> file { get; set; }
        public int userId { get; set; }
        public string hashUnic { get; set; }
        public string Flg_proceso { get; set; }

    }
}
