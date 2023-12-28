using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class MailWorkIntegrity
    {
        public string Correo { get; set; }
        public string Usuario { get; set; }
        public string Telefono { get; set; }
        public string NroSolicitud { get; set; }
        public string DesServicio { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Lastname2 { get; set; }
        public string EstadoSol { get; set; }
        public string keySubject { get; set; }
        public string Empresa { get; set; }
        public string keyBody { get; set; }
        public string[] SubjectText { get; set; }
        public string enlace { get; set; }
        public string NombreInforme { get; set; }
    }
}
