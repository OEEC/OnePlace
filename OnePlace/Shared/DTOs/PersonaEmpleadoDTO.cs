using OfficeOpenXml.Attributes;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class PersonaEmpleadoDTO
    {
        [DisplayName("Nombre/s")] public string Nombre { get; set; } = string.Empty;
        [DisplayName("Apellido paterno")] public string Apellido_Pat { get; set; } = string.Empty;
        [DisplayName("Apellido materno")] public string Apellido_Mat { get; set; } = string.Empty;
        [DisplayName("No. empleado")] public string NoEmpleado { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string CURP { get; set; } = string.Empty;
        public string NSS { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        [DisplayName("Correo de empleado")] public string Correo_Empleado { get; set; } = string.Empty;
        [DisplayName("Telefono de empleado")] public string Telefono_Empleado { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string Zona { get; set; } = string.Empty;
        [EpplusIgnore] public int? Id_Zona { get; set; }
        public string Pagadora { get; set; } = string.Empty;
        [EpplusIgnore] public int? Id_Pagadora { get; set; }
        public string Estacion { get; set; } = string.Empty;
        [EpplusIgnore] public int? Id_Estacion { get; set; }
        public string Departamento { get; set; } = string.Empty;
        [EpplusIgnore] public int? Id_Departamento { get; set; }
        public string Area { get; set; } = string.Empty;
        [EpplusIgnore] public int? Id_Area { get; set; }
        public string Puesto { get; set; } = string.Empty;
        [EpplusIgnore] public int? Id_Puesto { get; set; }
        [DisplayName("Nombre de usuario")] public string Nombre_Usuario { get; set; } = string.Empty;
        [DisplayName("Contraseña")] public string Password_Usuario { get; set; } = string.Empty;

        public Empleado Obtener_Empleado()
        {
            Empleado empleado = new()
            {
                Noemp = NoEmpleado,
                Nombre_usuario = Nombre_Usuario,
                Password_usuario = Password_Usuario,
                Idarea = Id_Area,
                Iddepartamento = Id_Departamento,
                Idestacion = Id_Estacion,
                Idpagadora = Id_Pagadora,
                Idpuesto = Id_Puesto,
                ZonaId = Id_Zona,
                Division = Division,
                Fchalta = DateTime.Now,
                Idestatus = "1",
                Img = string.Empty
            };
            return empleado;
        }

        public Persona Obtener_Persona()
        {
            Persona persona = new()
            {
                Nombre = Nombre,
                ApeMat = Apellido_Mat,
                ApePat = Apellido_Pat,
                Correo = Correo,
                Telefono = Telefono,
                Rfc = RFC,
                Curp = CURP,
                Nss = NSS
            };
            return persona;
        }
    }
}
