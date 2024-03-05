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
        public DateTime? FechaInicio { get; set; }
        private DateTime? _FechaFinal { get; set; } //debe ser private     
        public DateTime? FechaFinal
        {
            get
            {
                //se usa hasvalue para que no tire error de object reference null al usar value en la fecha
                if (FechaInicio.HasValue)
                {
                    //si en caso que se eligiera el 28 de febrero mas un mes daria 28 de marzo y eso esta mal por que marzo trae 31 dias
                    //con esto se soluciona y ahora suma correctamente un mes sea cual sea el dia en cualquier mes seleccionado
                    _FechaFinal = FechaInicio.Value.AddDays(1).AddMonths(1).AddDays(-1);                    
                }

                //se retorna aqui por que dentro del if no reconoce el return
                return _FechaFinal;
            }
            set
            {
                _FechaFinal = value;
            }
        }
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
        public TiendaoEstacion TiendaoEstacion { get; set; }
        [NotMapped] public List<Zona> Zonas { get; set; }
        [NotMapped] public List<CursoZona> CursoZonas { get; set; }
        [NotMapped] public List<Zona> ZonasSeleccionadas { get; set; } = new();
        [NotMapped] public List<Zona> ZonasNoSeleccionadas { get; set; } = new();
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
    public enum TiendaoEstacion
    {
        [Description("Estación")]
        Estacion,
        Tienda,
    }
}
