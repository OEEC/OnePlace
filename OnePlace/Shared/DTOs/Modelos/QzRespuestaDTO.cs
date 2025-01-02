using Newtonsoft.Json;
using System;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class QzRespuestaDTO
    {
        public int? PreguntaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string Respuesta { get; set; } = string.Empty;
        public DateTime? Fecha { get; set; }
        public QzPreguntaDTO Pregunta { get; set; } = null!;
        public UsuarioAspDTO Usuario { get; set; } = null!;
    }

    public class QzRespuestaSimpleDTO
    {
        public int? PreguntaId { get; set; }
        public string Respuesta { get; set; } = string.Empty;
        public DateTime? Fecha { get; set; }
        [JsonIgnore]
        public string UsuarioId { get; set; } = string.Empty;
    }
}
