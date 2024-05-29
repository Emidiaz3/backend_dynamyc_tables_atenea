namespace ApiRestCuestionario.Model
{
    public class ItemResp
    {
        public int status { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }
    public class ItemResplistobject
    {
        public int status { get; set; }
        public string? message { get; set; }
        public List<object> data { get; set; } = [];
    }
}
