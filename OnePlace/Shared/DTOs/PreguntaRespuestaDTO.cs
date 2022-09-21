using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class PreguntaRespuestaDTO
    {
        public Empleado Empleado { get; set; }
        public Tema Tema { get; set; }
        public List<Pregunta> ListadePreguntas { get; set; }
        public List<Respuesta> ListadeRespuestas { get; set; }
    }
}
