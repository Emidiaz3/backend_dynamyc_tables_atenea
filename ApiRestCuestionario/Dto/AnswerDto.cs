using ApiRestCuestionario.Model;
using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Dto
{
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
