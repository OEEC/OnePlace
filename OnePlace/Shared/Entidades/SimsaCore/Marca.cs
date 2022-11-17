using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Brand
    {
        [JsonProperty("idmarca")]
        public int Idmarca { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("marca")]
        public string Marca1 { get; set; }

        [JsonProperty("idempresa")]
        public int? Idempresa { get; set; }

        [JsonProperty("idtipo")]
        public int? Idtipo { get; set; }

        [JsonProperty("idestatus")]
        public int? Idestatus { get; set; }

        [JsonProperty("fchcreacion")]
        public DateTime? Fchcreacion { get; set; }

        [JsonProperty("fchmod")]
        public DateTime? Fchmod { get; set; }

        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }

        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }
    }
    public class ResultObjectBrand
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("marcas")]
        public List<Brand> ListadeMrcas { get; set; }
    }
    public partial class Marca
    {
        public int Idmarca { get; set; }
        public string Img { get; set; }
        public string Marca1 { get; set; }
        public int? Idempresa { get; set; }
        public int? Idtipo { get; set; }
        public int? Idestatus { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchmod { get; set; }
        public DateTime? Fchbaja { get; set; }
        public int? Idusuario { get; set; }
    }
}
