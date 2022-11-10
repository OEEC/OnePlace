using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Empleado
    {
        public int Idempleado { get; set; }      
        public string Img { get; set; }
        public string Noemp { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }       
        public double? Nomina { get; set; }
        public double? Variable { get; set; }
        public int? Idtipo { get; set; }
        public string Idestatus { get; set; }
        public DateTime? Fchalta { get; set; }
        public DateTime? Fchactualizado { get; set; }//simsa core osacar
        public int? Actualizado_por { get; set; }//simsa core osacar
        public DateTime? Fchbaja { get; set; }
        public int? Borrado_por { get; set; }//simsa core osacar

        //Sin el [NotMapped] atributo EF, se crearía una relación de uno a uno basada en la propiedad Persona de navegación
        //en Empleado la propiedad de clave externa idpersona en Persona.
        //El [NotMapped] impedirá eso.       
        public int? Idpersona { get; set; }
        [NotMapped]
        public virtual Persona Persona { get; set; }
        public int? Iddepartamento { get; set; }
        [NotMapped]
        public virtual Departamento Departamento { get; set; }       
        public int? Idarea { get; set; }
        [NotMapped]
        public virtual Area Area { get; set; }
        public int? Idpuesto { get; set; }
        [NotMapped]
        public virtual Puesto Puesto { get; set; }
        public int? Idpagadora { get; set; } //simsa core osacar
        [NotMapped]
        public virtual Pagadora Pagadora { get; set; }
        public int? ZonaId { get; set; }       
        public int? Idestacion { get; set; }
        public int? TiendaId { get; set; }
        public List<ImagenesCumpleEmpleado> ImagenesCumple { get; set; }
    }
    public class ImagenesCumpleEmpleado
    {
        public int ImagenesCumpleEmpleadoId { get; set; }
        public string Imagen { get; set; }
        public int EmpleadoId { get; set; }
    }
}
