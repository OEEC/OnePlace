using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    //clase para usar con la apirest
    public class Zone
    {
        //la jsonproperty es para hacer el match con el nombre original que viene en el json
        [JsonProperty("idzona")]
        public int ZonaId { get; set; }

        [JsonProperty("zona")]
        public string Zona1 { get; set; }

        [JsonProperty("idestatus")]
        public int? Idestatus { get; set; }

        [JsonProperty("fchcreacion")]
        public DateTime? Fchcreacion { get; set; }

        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }

        [JsonProperty("fchmod")]
        public DateTime? Fchmod { get; set; }

        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }
    }
    public class ResultObjectZone
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("zonas")]
        public List<Zona> Zonas { get; set; }
    }

    //clase para usar con el context
    public class Zona
    {
        [JsonProperty("idzona")]
        [NotMapped] public int Id_Zona { get; set; }
        public int ZonaId { get; set; }
        [JsonProperty("zona")]
        [NotMapped] public string Nombre_Zona { get; set; }
        public string Zona1 { get; set; }
        [JsonProperty("idestatus")]
        public int? Idestatus { get; set; }
        [JsonProperty("fchcreacion")]
        public DateTime? Fchcreacion { get; set; }
        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }
        [JsonProperty("fchmod")]
        public DateTime? Fchmod { get; set; }
        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }
        public List<PromocionZona> PromocionZona { get; set; }
        public List<CapacitacionContinuaZona> CapacitacionContinuaZona { get; set; }
    }
}
