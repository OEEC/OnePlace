using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class ZonaDTO
    {
        public int Id { get; set; }
        public string Zona { get; set; } = string.Empty;
    }
}
