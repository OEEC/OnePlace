using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.PokeModels
{
    public class Pokemon
    {
        //la jsonproperty es para hacer el match con el nombre original que viene en el json
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("base_experience")]
        public int Experience { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("types")]
        public List<Type> Types { get; set; }

        [JsonProperty("sprites")]
        public Sprite Sprite { get; set; }
    }   
}
