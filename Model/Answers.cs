namespace ApiRestCuestionario.Model
{
    public class Answers
    {
        public int id { get; set; }
        public string? answer { get; set; }
        public DateTime? answer_date { get; set; } 
        public int users_id { get; set; }
        public int questions_id { get; set; }
        public int form_id { get; set; }
        public string? hashUnic { get; set; }
        public string? Flg_proceso { get; set; }

    }
}
