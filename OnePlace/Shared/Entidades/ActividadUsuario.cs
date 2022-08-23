using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class ActividadUsuario
    {
        public int ActividadUsuarioId { get; set; }
        public string UserId { get; set; }//saber quien hizo la actividad y que el progreso sea por usuario
        public bool IsComplete { get; set; }//saber si ya termino alguna actividad dentro del curso
        public int TemaId { get; set; }//saber a que tema a la que pertenece la actividad
        public int FaseCursoId { get; set; }//saber a que fase pertenece la actividad       
    }
}
