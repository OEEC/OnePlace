using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Employee
    {
        [JsonProperty("idempleado")]
        public int Idempleado { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("noemp")]
        public string Noemp { get; set; }

        [JsonProperty("correo")]
        public string Correo { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [JsonProperty("nomina")]
        public double? Nomina { get; set; }

        [JsonProperty("variable")]
        public double? Variable { get; set; }

        [JsonProperty("idtipo")]
        public int? Idtipo { get; set; }

        [JsonProperty("idestatus")]
        public string Idestatus { get; set; }

        [JsonProperty("fchalta")]
        public DateTime? Fchalta { get; set; }

        [JsonProperty("fchactualizado")]
        public DateTime? Fchactualizado { get; set; }

        [JsonProperty("actualizado_por")]
        public int? Actualizado_por { get; set; }

        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }

        [JsonProperty("borrado_por")]
        public int? Borrado_por { get; set; }

        [JsonProperty("idpersona")]
        public int? Idpersona { get; set; }

        [JsonProperty("iddepartamento")]
        public int? Iddepartamento { get; set; }

        [JsonProperty("idarea")]
        public int? Idarea { get; set; }

        [JsonProperty("idpuesto")]
        public int? Idpuesto { get; set; }

        [JsonProperty("idpagadora")]
        public int? Idpagadora { get; set; } 

        [JsonProperty("idzona")]
        public int? ZonaId { get; set; }

        [JsonProperty("idestacion")]
        public int? Idestacion { get; set; }

        [JsonProperty("idtienda")]
        public int? TiendaId { get; set; }

        [JsonProperty("divicion")]
        public string? Division { get; set; }
      
    }
    public class ResultObjectEmployee
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("empleados")]
        public List<Employee> ListadeEmpleados { get; set; }
    }

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
        [NotMapped]
        public virtual Estacion Estacion { get; set; }
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
        public virtual Zona Zona { get; set; }  
        public int? Idestacion { get; set; }
        public int? TiendaId { get; set; }
        public string? Division { get; set; }
        public List<ImagenesCumpleEmpleado> ImagenesCumple { get; set; }
    }
    public class ImagenesCumpleEmpleado
    {
        public int ImagenesCumpleEmpleadoId { get; set; }
        public string Imagen { get; set; }
        public int EmpleadoId { get; set; }
    }
}
