using OnePlace.Shared.DTOs.Modelos;
using System;
using System.Collections.Generic;

namespace OnePlace.Shared.DTOs.Reportes
{
    public class QuizRespuestasRecomendacionDTO
    {
        public string EstacionTienda { get; set; }
        public List<QzPreguntaDTO> Preguntas { get; set; } = new();
        public DateTime Fecha { get; set; }
        public string Calificado { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
    }

    public class QuizPreguntasRecomendacionDTO
    {
        public List<QuizRespuestasRecomendacionDTO> Respuestas { get; set; } = new();
        public int CantidadPreguntas { get; set; } = 0;
    }
}
