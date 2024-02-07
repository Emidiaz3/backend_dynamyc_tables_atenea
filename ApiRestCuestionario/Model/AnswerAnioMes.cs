using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class AnswerAnioMes
    {
        public int id { get; set; }
        public int idForm { get; set; }
        public int idQuestion { get; set; }
        public int anio { get; set; }
        public string mes { get; set; }
        public double valor { get; set; }
        public string hashUnic { get; set; }
        public DateTime? answer_date { get; set; }
    }
}
