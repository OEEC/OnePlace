using Newtonsoft.Json;
using OnePlace.Shared.DTOs.Modelos;
using OnePlace.Shared.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public class ResultObjectEmployee
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("empleados")]
        //public List<Employee> ListadeEmpleados { get; set; }
        public List<Empleado> ListadeEmpleados { get; set; }
    }

    public partial class Empleado
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
        public DateTime? Fchactualizado { get; set; }//simsa core osacar
        [JsonProperty("actualizado_por")]
        public int? Actualizado_por { get; set; }//simsa core osacar
        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }
        [JsonProperty("borrado_por")]
        public int? Borrado_por { get; set; }//simsa core osacar

        //Sin el [NotMapped] atributo EF, se crearía una relación de uno a uno basada en la propiedad Persona de navegación
        //en Empleado la propiedad de clave externa idpersona en Persona.
        //El [NotMapped] impedirá eso.       
        [JsonProperty("idpersona")]
        public int? Idpersona { get; set; }
        [NotMapped]
        public virtual Persona Persona { get; set; }
        [NotMapped]
        public virtual Estacion Estacion { get; set; }
        [JsonProperty("iddepartamento")]
        public int? Iddepartamento { get; set; }
        [NotMapped]
        public virtual Departamento Departamento { get; set; }
        [JsonProperty("idarea")]
        public int? Idarea { get; set; }
        [NotMapped]
        public virtual Area Area { get; set; }
        [JsonProperty("idpuesto")]
        public int? Idpuesto { get; set; }
        [NotMapped]
        public virtual Puesto Puesto { get; set; }
        [JsonProperty("idpagadora")]
        public int? Idpagadora { get; set; } //simsa core osacar
        [NotMapped]
        public virtual Pagadora Pagadora { get; set; }
        [JsonProperty("idzona")]
        public int? ZonaId { get; set; }
        [NotMapped]
        public virtual Zona Zona { get; set; }
        [JsonProperty("idestacion")]
        public int? Idestacion { get; set; }
        [JsonProperty("idtienda")]
        public int? TiendaId { get; set; }
        [JsonProperty("divicion")]
        public string? Division { get; set; }
        public List<ImagenesCumpleEmpleado> ImagenesCumple { get; set; }
        [NotMapped]
        public string Obtener_Nombre_Usuario
        {
            get
            {
                var inizona = "NA";

                if (Zona is not null)
                    if (!string.IsNullOrEmpty(Zona.Zona1))
                        inizona = Zona.Zona1.ToUpper().Substring(0, 2);

                if (!string.IsNullOrEmpty(Noemp))
                    return Noemp.Trim() + inizona;

                return string.Empty;
            }
        }
        [NotMapped] public string Nombre_usuario { get; set; }
        [NotMapped] public string Password_usuario { get; set; }
        [NotMapped] public IdentityUsuario Usuario { get; set; } = null!;
        [NotMapped] public List<Estacion> Estaciones { get; set; } = new();
        [NotMapped] public List<EmpleadoEstacion> EmpleadoEstaciones { get; set; } = new();
        public string ObtenerEncargado()
        {
            if (!string.IsNullOrEmpty(Division))
            {
                if (Estacion.EmpleadoEstaciones.Any())
                {
                    int supervisorId = 0;
                    int jefeId = 0;
                    var supervisorEstacionId = 40;
                    var supervisorTiendaId = 77;
                    var jefeTiendaPuestoId = 214;
                    var jefeDeTurnoPuestoId = 32;
                    var gerenteadministrativo = 23;
                    var gerentePuestoIds = new List<int?>() { 24, 25, 26, 27, 28, 39, 49, 50, 139, 140, 141,
                                               142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                209, 210, 211, 212, 213, 219 };
                    var jefeadministrativo = 159;

                    if (Division == "ADMINISTRATIVO")
                    {
                        supervisorId = 242;
                        jefeId = 159;

                        if (Idpuesto == gerenteadministrativo)
                        {
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == supervisorId && x.EstacionId == Idestacion)?.Empleado.Persona.FullName;
                        }
                        else if (Idpuesto == jefeadministrativo)
                        {
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == gerenteadministrativo && x.EstacionId == Idestacion
                        && x.DepartamentoId == Iddepartamento)?.Empleado.Persona.FullName;
                        }
                        else
                        {
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == jefeadministrativo && x.EstacionId == Idestacion
                        && x.DepartamentoId == Iddepartamento)?.Empleado.Persona.FullName;
                        }
                    }

                    if (Division == "TIENDAS")
                    {
                        supervisorId = 77;
                        jefeId = 214;
                    }
                    else if (Division == "ESTACIONES")
                    {
                        supervisorId = 40;
                        jefeId = 32;
                    }

                    if (Idpuesto == supervisorEstacionId || Idpuesto == supervisorTiendaId)
                    {
                        return "VICTOR ARROYO ALONSO";
                    }
                    else if (gerentePuestoIds.Contains(Idpuesto))
                    {
                        //if(Estacion.Zona == 5)
                        //{
                        //    return "MAURICCIO VAZQUEZ SILVA";
                        //}
                        return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == supervisorId && !x.Esgerente && !x.Esjefeturno)?.Empleado?.Persona?.FullName;
                    }
                    else if (Idpuesto == jefeDeTurnoPuestoId || Idpuesto == jefeTiendaPuestoId)
                    {
                        var encargado = Estacion.EmpleadoEstaciones.FirstOrDefault(x => gerentePuestoIds.Contains(x.PuestoId) && x.Esgerente)?.Empleado?.Persona?.FullName;
                        if (Estacion.EmpleadoEstaciones.Any(x => x.EmpleadoId == Idempleado && x.Esgerente))
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == supervisorId && !x.Esgerente && !x.Esjefeturno)?.Empleado?.Persona?.FullName ?? encargado;
                        return encargado;
                    }
                    else
                    {
                        var encargado = Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == jefeId && x.Esjefeturno)?.Empleado?.Persona?.FullName;

                        encargado ??= Estacion.EmpleadoEstaciones.FirstOrDefault(x => gerentePuestoIds.Contains(x.PuestoId) && x.Esgerente)?.Empleado?.Persona?.FullName;

                        return encargado ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }

        public string ObtenerPuestoEncargado()
        {
            if (!string.IsNullOrEmpty(Division))
            {
                if (Estacion.EmpleadoEstaciones.Any())
                {
                    int supervisorId = 0;
                    int jefeId = 0;
                    var jefeTiendaPuestoId = 214;
                    var jefeDeTurnoPuestoId = 32;
                    var gerenteadministrativo = 23;
                    var gerentePuestoIds = new List<int?>() { 24, 25, 26, 27, 28, 39, 49, 50, 139, 140, 141,
                                               142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                209, 210, 211, 212, 213, 219 };
                    var jefeadministrativo = 159;
                    if (Division == "ADMINISTRATIVO")
                    {
                        supervisorId = 242;
                        jefeId = 159;

                        if (Idpuesto == gerenteadministrativo)
                        {
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == supervisorId && x.EstacionId == Idestacion)?.Puesto?.Puesto1;
                        }
                        else if (Idpuesto == jefeadministrativo)
                        {
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == gerenteadministrativo && x.EstacionId == Idestacion
                            && x.DepartamentoId == Iddepartamento)?.Puesto?.Puesto1;
                        }
                        else
                        {
                            return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == jefeadministrativo && x.EstacionId == Idestacion
                            && x.DepartamentoId == Iddepartamento)?.Puesto?.Puesto1;
                        }
                    }

                    if (Division == "TIENDAS")
                    {
                        supervisorId = 77;
                        jefeId = 214;
                    }
                    else if (Division == "ESTACIONES")
                    {
                        supervisorId = 40;
                        jefeId = 32;
                    }

                    if (gerentePuestoIds.Contains(Idpuesto))
                    {
                        return Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == supervisorId && !x.Esgerente && !x.Esjefeturno)?.Puesto?.Puesto1;
                    }
                    else if (Idpuesto == jefeDeTurnoPuestoId || Idpuesto == jefeTiendaPuestoId)
                    {
                        var encargado = Estacion.EmpleadoEstaciones.FirstOrDefault(x => gerentePuestoIds.Contains(x.PuestoId) && x.Esgerente)?.Puesto?.Puesto1;
                        if (Estacion.EmpleadoEstaciones.Any(x => x.EmpleadoId == Idempleado && x.Esgerente))
                            encargado ??= Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == supervisorId && !x.Esgerente && !x.Esjefeturno)?.Puesto?.Puesto1;
                        return encargado;
                    }
                    else
                    {
                        var encargado = Estacion.EmpleadoEstaciones.FirstOrDefault(x => x.PuestoId == jefeId && x.Esjefeturno)?.Puesto?.Puesto1;
                        encargado ??= Estacion.EmpleadoEstaciones.FirstOrDefault(x => gerentePuestoIds.Contains(x.PuestoId) && x.Esgerente)?.Puesto?.Puesto1;
                        return encargado ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }
    }
    public class ImagenesCumpleEmpleado
    {
        public int ImagenesCumpleEmpleadoId { get; set; }
        public string Imagen { get; set; }
        public int EmpleadoId { get; set; }
    }
}
