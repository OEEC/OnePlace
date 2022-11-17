using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.PokeModels
{
    public class ResultObject
    {
        [JsonProperty("results")]
        public List<Pokemon> Pokemons { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
