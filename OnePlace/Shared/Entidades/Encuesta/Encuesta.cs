using System;

namespace OnePlace.Shared.Entidades.Encuesta
{
    public class Encuesta
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaCaducidad { get; set; } = DateTime.Today;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }
}
