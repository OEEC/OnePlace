using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class AreaDos
    {
        [JsonProperty("idarea")]
        public int Idarea { get; set; }

        [JsonProperty("iddepartamento")]
        public int? Iddepartamento { get; set; }

        [JsonProperty("area")]
        public string Area1 { get; set; }

        [JsonProperty("fchcreacion")]
        public DateTime? Fchcreacion { get; set; }

        [JsonProperty("fchmod")]
        public DateTime? Fchmod { get; set; }

        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }

        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }
    }

    public class ResultObjectAreaDos
    {
        [JsonProperty("areas")]
        public List<Area> ListadeAreas { get; set; }
    }

    public partial class Area
    {
        [JsonProperty("idarea")]
        public int Idarea { get; set; }
        [JsonProperty("iddepartamento")]
        public int? Iddepartamento { get; set; }
        [NotMapped, JsonProperty("area")]
        public string Nombre_Area { get; set; }
        public string Area1 { get; set; }
        [JsonProperty("fchcreacion"), JsonIgnore]
        public DateTime? Fchcreacion { get; set; }
        [JsonProperty("fchmod"), JsonIgnore]
        public DateTime? Fchmod { get; set; }
        [JsonProperty("fchbaja"), JsonIgnore]
        public DateTime? Fchbaja { get; set; }
        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }
    }
}
