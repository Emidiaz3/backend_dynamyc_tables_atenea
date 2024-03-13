using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class ReporteFinal
    {
        public int id { get; set; }
        public double Año { get; set; }

        public double Numes { get; set; }
        
        public string Mes { get; set; }
    }
}
