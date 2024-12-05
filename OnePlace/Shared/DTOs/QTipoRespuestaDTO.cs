using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class QTipoRespuestaDTO
    {
        public int IdTipoRespuesta { get; set; }
        public int? TipoPreguntaId { get; set; }
        public int? Cantidad { get; set; }
        public string? Texto { get; set; }
        public QTipoPreguntaDTO? TipoPregunta { get; set; }
    }
}
