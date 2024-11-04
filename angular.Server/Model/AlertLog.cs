using System;

namespace angular.Server.Model
{
    public class AlertLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string Response { get; set; }
    }
}
