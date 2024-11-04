using System.ComponentModel.DataAnnotations;

namespace angular.Server.Model
{
    public class SendingPhone
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }  // Asegúrate de que esta propiedad esté definida
        public Guid ClientId { get; set; }
    }

    public class ReceivingPhone
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }




}
