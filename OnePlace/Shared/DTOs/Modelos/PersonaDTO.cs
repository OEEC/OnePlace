using Newtonsoft.Json;

namespace OnePlace.Shared.DTOs.Modelos
{
    public class PersonaDTO
    {
        public int Id { get; set; }
        public string Ape_pat { get; set; }
        public string Ape_mat { get; set; }
        public string Nombre { get; set; }
        public string FullName
        {
            get { return $"{Nombre?.Trim()} {Ape_pat?.Trim()} {Ape_mat?.Trim()}"; }
        }
    }
}
