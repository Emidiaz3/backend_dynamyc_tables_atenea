using ApiRestCuestionario.Model;
using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Dto
{
    public class SaveMasiveAnswerDto
    {
        [Required]
        public int FormId { get; set; }

        [Required]
        public required string Data { get; set; }

    }
    public  class SaveAnswerDTO
    {
        [Required]
        public int FormId { get; set; }

        [Required]
        public string? Data { get; set; }

        [Required]
        public List<AnswerAnioMes>? listDataAnioMes { get; set; }
    }
}
