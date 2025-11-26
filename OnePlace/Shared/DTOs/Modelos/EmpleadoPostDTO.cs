using System;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class EmpleadoPostDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPat { get; set; } = string.Empty;
        public string ApellidoMat { get; set; } = string.Empty;
        public string NoEmpleado { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int IdDepartamento { get; set; }
        public int IdEstacion { get; set; }
        public int IdPuesto { get; set; }
        public int IdZona { get; set; }
        public string Division { get; set; } = string.Empty;
        public DateTime Fchalta { get; set; } = DateTime.Now;
    }
}
