

namespace ApiRestCuestionario.Dto
{
    public class SaveFormDocumentDto
    {
        public int formId { get; set; }
        public int questionsId { get; set; }
        public string db_name { get; set; }
        public required List<IFormFile> file { get; set; }
    }
}
