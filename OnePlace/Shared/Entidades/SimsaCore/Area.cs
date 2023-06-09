﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public List<AreaDos> ListadeAreas { get; set; }
    }

    public partial class Area
    {
        public int Idarea { get; set; }
        public int? Iddepartamento { get; set; }
        public string Area1 { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchmod { get; set; }
        public DateTime? Fchbaja { get; set; }
        public int? Idusuario { get; set; }
    }
}
