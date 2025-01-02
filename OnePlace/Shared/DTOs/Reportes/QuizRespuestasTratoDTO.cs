using OnePlace.Shared.DTOs.Modelos;
using System.Collections.Generic;

namespace OnePlace.Shared.DTOs.Reportes
{
    public class QuizRespuestasTratoDTO
    {
        public string EstacionTienda { get; set; } = string.Empty;
        public int Suma { get; set; } = 0;
        public string Encargado { get; set; } = string.Empty;
        public List<int> Respuestas { get; set; } = new();
    }

    public record QuizPreguntasTratoDTO
    {
        public List<string> Preguntas { get; set; } = new();
        public List<QuizRespuestasTratoDTO> Respuestas { get; set; } = new();
    }
}
