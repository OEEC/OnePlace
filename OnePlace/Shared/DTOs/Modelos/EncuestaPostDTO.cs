using System;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class EncuestaPostDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaCaducidad { get; set; } = DateTime.Today;
    }
}
