using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Empresa_Form
    {
        
    }

    public class entidad_guardar_empresa
    {
        //[Key]
        public int IdEmpresa { get; set; }
        public int IdPais { get; set; }
        public int IdTipodocumentoEmpresa { get; set; }
        public string NroDocumento { get; set; }
        public string DescripcionEmpresa { get; set; }
        public int FlgEstado { get; set; }
        public int IdUsuarioAccion { get; set; }
        public string Direccion { get; set; }
        
    }

    public class entidad_lst_empresa
    {
        [Key]
        public int IdEmpresa { get; set; }
        public int IdPais { get; set; }
        public string Pais { get; set; }
        public int? IdTipodocumentoEmpresa { get; set; }
        public string DesTipoDocIdentidad { get; set; }
        public string NroDocumento { get; set; }
        public string DescripcionEmpresa { get; set; }
        public int? Flg_Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? IdUsuarioRegistra { get; set; }
        public int? IdUsuarioModificacion { get; set; }
        public string Direccion { get; set; }

    }
}
