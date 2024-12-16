using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class QPreguntaConRespuestasDTO
    {
        public int Idpregunta { get; set; }
        public int? GrupoId { get; set; }
        public string? Pregunta { get; set; }
        public int? TipoPreguntaId { get; set; }
        public List<QRespuestaDTO>? RespuestaUsuario { get; set; }
        public List<QTipoRespuestaDTO>? TipoRespuestas { get; set; }
    }
}
