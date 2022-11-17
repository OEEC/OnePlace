using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.PokeModels
{
    //dentro del array de la clase Pokemon llamado Type vienen dos propiedades una sola y un nuevo array o subnivel
    public class Type
    {
        [JsonProperty("slot")]
        public int Slot { get; set; }// propiedad sola
        [JsonProperty("type")]
        public PokemonType PokemonType { get; set; } //nuevo array o subnivel
    }
    public class PokemonType
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
