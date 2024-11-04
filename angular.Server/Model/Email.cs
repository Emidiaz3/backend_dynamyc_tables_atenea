namespace angular.Server.Model
{
    public class Email
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
