using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
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
}
