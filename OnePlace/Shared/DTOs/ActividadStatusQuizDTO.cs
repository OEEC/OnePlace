using OnePlace.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class ActividadStatusQuizDTO
    {
        public ActividadUsuarioQuiz ActividadUsuarioQuiz { get; set; }
        public EstadosdelQuiz EstadosdelQuiz { get; set; }
        public int TotaldePreguntas { get; set; }
        public int TotalRespuestasCorrectas { get; set; }
        public int Porcentaje { get; set; }
    }
}
