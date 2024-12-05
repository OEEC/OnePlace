using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class QRespuestaDTO
    {
        public int? PreguntaId { get; set; }
        public string? UsuarioId { get; set; }
        public string? Respuesta { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
