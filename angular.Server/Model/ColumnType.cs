using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestCuestionario.Model
{
    [Table("column_types")]
    public class ColumnType
    {
        public int Id { get; set; }
        public string? nombre_columna_db { get; set; }
        public string? nombre_columna_fronted { get; set; }
        public string? Datatype { get; set; }
        public string? props_ui { get; set; }
        public int form_id { get; set; }
        public int State { get; set; }
        public int? question_type_id { get; set; }
        public string? nombre_columna_db_2 { get; set; }
        public string? Datatype_2 { get; set; }

    }
}
