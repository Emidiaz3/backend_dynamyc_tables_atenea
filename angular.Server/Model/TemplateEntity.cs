using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestCuestionario.Model
{
    [Table("Templates")]
    public class TemplateEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Content { get; set;}

    }
}
