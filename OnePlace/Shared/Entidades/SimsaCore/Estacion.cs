using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Station
    {
        [Key]

        [JsonProperty("idestacion")]
        public int Idestacion { get; set; }

        [JsonProperty("codgas")]
        public int? Codgas { get; set; }

        [JsonProperty("nomcg")]
        public string Nomcg { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("permisocre")]
        public string Permisocre { get; set; }

        [JsonProperty("noest")]
        public string Noest { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("marca")]
        public int? Marca { get; set; }

        [JsonProperty("correo")]
        public string Correo { get; set; }

        [JsonProperty("tel")]
        public long? Tel { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("razonsocial")]
        public int? Razonsocial { get; set; }

        [JsonProperty("calle")]
        public string Calle { get; set; }

        [JsonProperty("noint")]
        public string Noint { get; set; }

        [JsonProperty("noext")]
        public string Noext { get; set; }

        [JsonProperty("colonia")]
        public string Colonia { get; set; }

        [JsonProperty("municipio")]
        public string Municipio { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("cp")]
        public string Cp { get; set; }

        [JsonProperty("zona")]
        public int? Zona { get; set; }

        [JsonProperty("latitud")]
        public double? Latitud { get; set; }

        [JsonProperty("longitud")]
        public double? Longitud { get; set; }

        [JsonProperty("qr")]
        public string Qr { get; set; }

        [JsonProperty("fchcreacion")]
        public string Fchcreacion { get; set; }

        [JsonProperty("fchmodificacion")]
        public string Fchmodificacion { get; set; }

        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }

        [JsonProperty("estatus")]
        public int? Estatus { get; set; }

        [JsonProperty("idrazonsocial")]
        public int? Idrazonsocial { get; set; }
    }
    public class ResultObjectStation
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("estaciones")]
        public List<Estacion> ListadeEstaciones { get; set; }
    }
    public partial class Estacion
    {
        [Key]
        public int Idestacion { get; set; }
        public int? Codgas { get; set; }
        public string Nomcg { get; set; }
        public string Img { get; set; }
        public string Permisocre { get; set; }
        public string Noest { get; set; }
        public string Nombre { get; set; }
        public int? Marca { get; set; }
        public string Correo { get; set; }
        public long? Tel { get; set; }
        public string Url { get; set; }
        public int? Razonsocial { get; set; }
        public string Calle { get; set; }
        public string Noint { get; set; }
        public string Noext { get; set; }
        public string Colonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Cp { get; set; }
        public int? Zona { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string Qr { get; set; }
        [JsonIgnore] public string Fchcreacion { get; set; }
        [JsonIgnore] public string Fchmodificacion { get; set; }
        public int? Idusuario { get; set; }
        public int? Estatus { get; set; }
        public int? Idrazonsocial { get; set; }

        //propiedades de navegacion
        [NotMapped] public List<Empleado> Empleados { get; set; }
    }
}
