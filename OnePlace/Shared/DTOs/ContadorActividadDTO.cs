using OnePlace.Shared.Filtros;
using System;

namespace OnePlace.Shared.DTOs
{
    public class ContadorActividadDTO : Parametros_Busqueda_Gen
    {
        public string Estacion { get; set; } = string.Empty;
        public string Empleado { get; set; } = string.Empty;
        public string Persona { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public DateTime Fecha_Registro { get; set; } = DateTime.Now;
    }
}
