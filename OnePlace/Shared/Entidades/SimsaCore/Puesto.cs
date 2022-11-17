using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Position
    {
        [JsonProperty("idpuesto")]
        public int Idpuesto { get; set; }

        [JsonProperty("puesto")]
        public string Puesto1 { get; set; }

        [JsonProperty("fchcreacion")]
        public DateTime? Fchcreacion { get; set; }

        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }

        [JsonProperty("fchmod")]
        public DateTime? Fchmod { get; set; }

        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }
    }

    public class ResultObjectPosition
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("puestos")]
        public List<Position> ListadePuestos { get; set; }
    }
    public partial class Puesto
    {
        public int Idpuesto { get; set; }
        public string Puesto1 { get; set; }   
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
    }
}
