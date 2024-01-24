using ApiRestCuestionario.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Dto
{
    public class SaveMasiveAnswerDto
    {
        [Required]
        public int FormId { get; set; }

        [Required]
        public string Data { get; set; }

    }
    public  class SaveAnswerDTO
    {
        [Required]
        public int formId { get; set; }

        [Required]
        public List<Answers> dataAnswer { get; set; }

        [Required]
        public List<AnswerAnioMes> listDataAnioMes { get; set; }
    }
}
