using Newtonsoft.Json;
using OnePlace.Shared.Entidades;
using System.Collections.Generic;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class QzPreguntaDTO
    {
        public int Idpregunta { get; set; }
        public int? GrupoId { get; set; }
        public string Pregunta { get; set; } = string.Empty;
        public int? TipoPreguntaId { get; set; }
        public QzGrupoDTO Grupo { get; set; } = null!;
        public QzTipoPreguntaDTO TipoPregunta { get; set; } = null!;
        public List<QzRespuestaSimpleDTO> ListaRepuesta { get; set; } = new();
        //public List<QzRespuestaDTO> ListaRepuesta { get; set; } = new();
        public List<QzTipoRespuestaDTO> ListTipoRspuesta { get; set; } = new();
        public List<QPreguntaTipoRespuesta> ListPreguntaTipoRespuesta { get; set; } = new();
        [JsonIgnore]
        public int Estatus { get; set; }

        [JsonIgnore]
        public string UsuarioId { get; set; } = string.Empty;
    }
}
