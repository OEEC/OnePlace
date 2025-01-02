using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class QTipoRespuesta
    {
        [Key]
        public int IdTipoRespuesta { get; set; }
        public int? PreguntaId { get; set; }
        public int? TipoPreguntaId { get; set; }
        public int? Cantidad { get; set; }
        public string? Texto { get; set; }
        [NotMapped]
        public QPreguntas? Pregunta { get; set; }
        [NotMapped]
        public QTipoPregunta? TipoPregunta { get; set; }
        public List<QPreguntas>? PreguntaList { get; set; } = null;
        public List<QPreguntaTipoRespuesta>? ListPreguntaTipoRespuesta { get; set; } = null;

    }
}
