using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int? TemaId { get; set; }//saber a que tema a la que pertenece la actividad
        public int FaseCursoId { get; set; }//saber a que fase pertenece la actividad                                           
        public int? Idempleado { get; set; }
        public Tema Tema { get; set; }
        public FaseCurso FaseCurso { get; set; }

        //el not mapped hace que la bd no quiera poner foreign key con la tabla empleados , no se puede por que una tabla es innodb y la otra myasam
        [NotMapped]
        public Empleado Empleado { get; set; }
    }
}
