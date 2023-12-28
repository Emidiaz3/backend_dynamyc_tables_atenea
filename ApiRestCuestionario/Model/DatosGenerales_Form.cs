using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestCuestionario.Model
{
    public class DatosGenerales_Form
    {
    }

    public class entidad_lst_pais
    {
        [Key]
        public int IdPais { get; set; }
        public string Descripcion { get; set; }
        public int IdIdioma { get; set; }
    }
    public class entidad_lst_tipodoc
    {
        [Key]
        public int IdTipoDocIdentidad { get; set; }
        public int IdPais { get; set; }
        public string Pais { get; set; }
        public string DesTipoDocIdentidad { get; set; }
        public int FlgEstado { get; set; }
    }
    public class entidad_lst_dep
    {
        [Key]
        public int IdDep { get; set; }
        public int IdPais { get; set; }
        public string CodDep { get; set; }
        public string DescDep { get; set; }
    }

    public class entidad_lst_prov
    {
        [Key]
        public int IdProv { get; set; }
        public int IdDep { get; set; }
        public string CodProv { get; set; }
        public string DescProv { get; set; }
    }

    public class entidad_lst_dist
    {
        [Key]
        public int IdDist { get; set; }
        public int IdProv { get; set; }
        public string CodDist { get; set; }
        public string DescDist { get; set; }
    }
}
