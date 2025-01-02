using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class QPreguntas
    {
        [Key]
        public int Idpregunta { get; set; }
        public int? GrupoId { get; set; }
        public string? Pregunta { get; set; }
        public int? TipoPreguntaId { get; set; }
        public int Estatus { get; set; } = 1; // Valor predeterminado
        [NotMapped]
        // Relación con QGrupo
        public QGrupo? Grupo { get; set; }
        [NotMapped]
        // Relación con QTipoPregunta
        public QTipoPregunta? TipoPregunta { get; set; }
        [NotMapped]
        // Relación con QRespuesta
        public List<QRespuesta>? ListaRepuesta { get; set; }
        [NotMapped]
        public QTipoRespuesta? TipoRespuestas { get; set; }
        [NotMapped]
        public List<QTipoRespuesta>? ListTipoRspuesta { get; set; } = null;
        [NotMapped]
        public List<QPreguntaTipoRespuesta>? ListPreguntaTipoRespuesta { get; set; } = null;
    }
}
