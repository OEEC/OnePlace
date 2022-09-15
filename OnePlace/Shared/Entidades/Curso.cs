using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    public class Curso
    {
        public int CursoId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public virtual ICollection<Tema> LisadeTemas { get; set; }
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
    }
    public class Tema
    {
        public int TemaId { get; set; }

        //se validan desde el server
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        //[Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int ArchivoId { get; set; }
        public int VideoId { get; set; }//ArchivoId de la tabla ArchivoAdjunto
        public string DescripcionVideo { get; set; }
        public string Imagen { get; set; }
        public bool Activo { get; set; }

        //propiedades de navegacion
        public int CursoId { get; set; }
        public virtual Curso Curso { get; set; }
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
        public List<FaseCurso> FaseCurso { get; set; } = new List<FaseCurso>();
    }

    //guardar el estado del curso por usuario, define si un curso ya fue terminado
    public class CursoEstado
    {
        public int CursoEstadoId { get; set; }
        public int? CursoId { get; set; }
        public string UserId { get; set; }
        public int? Idempleado { get; set; }
        public EstadoCurso EstadoCurso { get; set; }
        public Curso Curso { get; set; }
        //el not mapped hace que la bd no quiera poner foreign key con la tabla empleados , no se puede por que una tabla es innodb y la otra myasam
        [NotMapped]
        public Empleado Empleado { get; set; }
    }
    public enum EstadoCurso
    {
        [Description("Sin Completar")]
        SinCompletar,
        Terminado,
              
    }
}
