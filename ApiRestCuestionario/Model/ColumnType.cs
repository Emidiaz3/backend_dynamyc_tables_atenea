namespace ApiRestCuestionario.Model
{
    public class ColumnType
    {
        public int id { get; set; }
        public string nombre_columna_db { get; set; }
        public string nombre_columna_fronted { get; set; }
        public string datatype { get; set; }
        public string props_ui { get; set; }
        public int form_id { get; set; }

    }
}
