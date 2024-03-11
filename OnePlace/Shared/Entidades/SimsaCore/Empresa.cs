using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Company
    {
        [JsonProperty("idempresa")]
        public int Idempresa { get; set; }

        [JsonProperty("rfc")]
        public string Rfc { get; set; }

        [JsonProperty("patronal")]
        public string Patronal { get; set; }

        [JsonProperty("razonsocial")]
        public string Razonsocial { get; set; }

        [JsonProperty("domicilio")]
        public string Domicilio { get; set; }

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
    public class ResultObjectCompany
    {
        //esto en el json es un array y adentro contiene mas propiedades esta por jerarquia de niveles
        [JsonProperty("razonesSociales")]
        public List<Empresa> ListadeRazonesSociales { get; set; }
    }
    public partial class Empresa
    {
        public int Idempresa { get; set; }
        public string Rfc { get; set; }
        public string Patronal { get; set; }
        public string Razonsocial { get; set; }
        public string Domicilio { get; set; }
        public int? Idestatus { get; set; }
        [JsonIgnore]
        public DateTime? Fchcreacion { get; set; }
        [JsonIgnore]
        public DateTime? Fchmod { get; set; }
        [JsonIgnore]
        public DateTime? Fchbaja { get; set; }
        public int? Idusuario { get; set; }

        [NotMapped] public ICollection<AreaDepartamentoEmpresa> AreaDepartamentoEmpresas { get; set; }
    }
}
