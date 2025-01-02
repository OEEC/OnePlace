using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class QPreguntaTipoRespuesta
    {
        public int TipoRespuestaId { get; set; }
        public int PreguntaId { get; set; }
        [NotMapped]
        public QTipoRespuesta? TipoRespuesta { get; set; } = null;
        [NotMapped]
        public QPreguntas? Pregunta { get; set;} = null;
    }
}
