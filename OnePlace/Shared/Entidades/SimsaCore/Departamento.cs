using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Departments
    {
        [JsonProperty("iddepartamento")]
        public int Iddepartamento { get; set; }

        [JsonProperty("idempresa")]
        public int? Idempresa { get; set; }

        [JsonProperty("departamento")]
        public string Departamento1 { get; set; }

        [JsonProperty("fchcreacion")]
        public DateTime? Fchcreacion { get; set; }

        [JsonProperty("fchbaja")]
        public DateTime? Fchbaja { get; set; }

        [JsonProperty("fchmod")]
        public DateTime? Fchmod { get; set; }

        [JsonProperty("idusuario")]
        public int? Idusuario { get; set; }

        [JsonProperty("idestatus")]
        public int? Idestatus { get; set; }//este campo no esta en la bd de oneplace
    }

    public class ResultObjectDepartments
    {       
        [JsonProperty("departamentos")]
        public List<Departments> ListadeDepartamentos { get; set; }
    }

    public partial class Departamento
    {
        public int Iddepartamento { get; set; }
        public int? Idempresa { get; set; }
        public string Departamento1 { get; set; }
        public DateTime? Fchcreacion { get; set; }
        public DateTime? Fchbaja { get; set; }
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
    }
}
