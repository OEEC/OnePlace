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
        public string Division { get; set; } = string.Empty;
        public string Zona { get; set; } = string.Empty;
        public string Estacion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        [DisplayName("Nombre de usuario")] public string Nombre_Usuario { get; set; } = string.Empty;
        [DisplayName("Contraseña")] public string Password_Usuario { get; set; } = string.Empty;
    }
}
