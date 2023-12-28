using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class Sucursal_Form
    {
    }

    public class entidad_guardar_sucursal
    {
        public int IdSucursal { get; set; }
        public int IdEmpresa { get; set; }
        public int IdPais { get; set; }
        public string DescripcionSucursal { get; set; }
        public string Direccion { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public int IdUsuarioAccion { get; set; }
        public int FlgEstado { get; set; }       
    }

    public class entidad_lst_sucursal
    {
        [Key]
        public int IdSucursal { get; set; }
        public int IdEmpresa { get; set; }
        public string DescripcionEmpresa { get; set; }
        public int IdPais { get; set; }
        public string Pais { get; set; }
        public string DescripcionSucursal { get; set; }          
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAccion { get; set; }
        public int? IdUsuarioCreacion { get; set; }
        public int? IdUsuarioAccion { get; set; }
        public int? Flg_Estado { get; set; }
        public string Direccion { get; set; }

    }
}
