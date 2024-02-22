using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnePlace.Shared.Entidades
{
    public class Quiz
    {
        public int QuizId { get; set; }

        //[Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        //[Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public List<Pregunta> LisadePreguntas { get; set; } = new List<Pregunta>();
        public string NombreCortado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    return null;
                }

                if (Nombre.Length > 60)
                {
                    return Descripcion.Substring(0, 60) + "...";
                }
                else
                {
                    return Nombre;
                }
            }
        }
        public string DescripcionCortada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Descripcion))
                {
                    return null;
                }

                if (Descripcion.Length > 60)
                {
                    return Descripcion.Substring(0, 60) + "...";
                }
                else
                {
                    return Descripcion;
                }
            }
        }      
        public int? TemaId { get; set; }
        public virtual Tema Tema { get; set; }
    }
    public class Pregunta
    {
        public int PreguntaId { get; set; }
        public string NombrePregunta { get; set; }
        public string PreguntaRespuesta { get; set; }
        public List<PalabrasClave> PalabrasClave { get; set; } = new List<PalabrasClave>();
        public DateTime? FechaRegistro { get; set; }
        public bool Activo { get; set; }

        //propiedades de navegacion
        public virtual Quiz Quiz { get; set; }
    }
    public class Respuesta
    {
        public int RespuestaId { get; set; }
        public string NombreRespuesta { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool Correcta { get; set; }
        public bool Activo { get; set; }
        public int? PreguntaId { get; set; }
        public virtual Pregunta Pregunta { get; set; }
    }
    public class PalabrasClave
    {
        public int PalabrasClaveId { get; set; }
        public string Palabra { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechadeRegistro { get; set; }
    }
    public class EstadosdelQuiz
    {
        public int EstadosdelQuizId { get; set; }
        public Evaluacion Evaluacion { get; set; }
        public EstadoQuiz EstadoQuiz { get; set; }
    }   
    public class ActividadUsuarioQuiz
    {
        public int ActividadUsuarioQuizId { get; set; }
        public string UserId { get; set; }  
        public int QuizId { get; set; }
        public int EstadosdelQuizId { get; set; }
        public int? Idempleado { get; set; }
        public Quiz Quiz { get; set; }
        public EstadosdelQuiz EstadosdelQuiz { get; set; }

        //el not mapped hace que la bd no quiera poner foreign key con la tabla empleados , no se puede por que una tabla es innodb y la otra myasam
        [NotMapped]
        public Empleado Empleado { get; set; }

        [NotMapped]
        public String NombreEmpleado { get; set; } 
    }
    public enum Evaluacion
    {
        Aprobado,
        Reprobado
    }
    public enum EstadoQuiz
    {
        Comenzado,
        Pendiente,
        Terminado
    }
}
