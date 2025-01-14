using Newtonsoft.Json;
using System.Collections.Generic;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class QzPreguntaDTO
    {
        public string Empleado { get; set; } = string.Empty;
        public int Idpregunta { get; set; }
        public int? GrupoId { get; set; }
        public string Pregunta { get; set; } = string.Empty;
        public int? TipoPreguntaId { get; set; }
        public List<QzRespuestaSimpleDTO> ListaRepuesta { get; set; } = new();
        [JsonIgnore]
        public int Estatus { get; set; }

        [JsonIgnore]
        public string UsuarioId { get; set; } = string.Empty;
    }
}
