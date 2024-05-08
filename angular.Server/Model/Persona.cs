using System.ComponentModel.DataAnnotations;

namespace ApiRestCuestionario.Model
{
    public class T_MAE_PERSONA
    {
        [Key]
        public int? IdPersona { get; set; }
        public string? ApellidosNombres { get; set; }
        public string? TipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? COD_MIGRACION { get; set; }
        public DateTime? fecharegistro { get; set; }
        public bool fgl_estado { get; set; }
        public int? flg_proceso { get; set; }
        public int? idusuario { get; set; }
        public int? fk_tipoPersona { get; set; }
        public int? fk_cargo { get; set; }
        public int? fk_localidad { get; set; }
        public int? fk_organizacion { get; set; }
        public string? TipoEncuesta { get; set; }

    }
}
