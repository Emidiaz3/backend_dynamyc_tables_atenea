namespace ApiRestCuestionario.Model
{
    public class Documents
    {
        public int id {  get; set; }
        public string? name { get; set; }
        public string? file_path { get; set; }
        public int user_id { get; set; }
        public int form_id { get; set; }
        public DateTime? created_at {  get; set; }

    }
}
