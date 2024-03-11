using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public List<Departamento> ListadeDepartamentos { get; set; }
    }

    public partial class Departamento
    {
        public int Iddepartamento { get; set; }
        public int? Idempresa { get; set; }
        [JsonProperty("departamento"), NotMapped]
        public string Nombre_Departamento { get; set; }
        public string Departamento1 { get; set; }
        [JsonIgnore]
        public DateTime? Fchcreacion { get; set; }
        [JsonIgnore]
        public DateTime? Fchbaja { get; set; }
        [JsonIgnore]
        public DateTime? Fchmod { get; set; }
        public int? Idusuario { get; set; }
        public int? Idestatus { get; set; }
        [NotMapped] public ICollection<AreaDepartamentoEmpresa> AreaDepartamentoEmpresas { get; set; }

        //public virtual ICollection<AreaDepartamentoEmpresa> AreaDepartamentoEmpresas { get; set; }
    }
}
