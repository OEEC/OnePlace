using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class QPreguntaRespuestaDTO
    {
        public Empleado Empleado { get; set; }
        public Tema Tema { get; set; }
        public List<QuizPregunta> ListadePreguntas { get; set; }
        public List<Respuesta> ListadeRespuestas { get; set; }
    }
}
