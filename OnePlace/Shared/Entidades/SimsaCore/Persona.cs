﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePlace.Shared.Entidades.SimsaCore
{
    public partial class Person
    {
        [JsonProperty("idpersona")]
        public int Idpersona { get; set; }

        [JsonProperty("ape_pat")]
        public string ApePat { get; set; }

        [JsonProperty("ape_mat")]
        public string ApeMat { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("sexo")]
        public string Sexo { get; set; }

        [JsonProperty("fchnac")]
        public DateTime? Fchnac { get; set; } = DateTime.MinValue;

        [JsonProperty("rfc")]
        public string Rfc { get; set; }

        [JsonProperty("curp")]
        public string Curp { get; set; }

        [JsonProperty("nss")]
        public string Nss { get; set; }

        [JsonProperty("calle")]
        public string Calle { get; set; }

        [JsonProperty("noext")]
        public string Noext { get; set; }

        [JsonProperty("noint")]
        public string Noint { get; set; }

        [JsonProperty("colonia")]
        public string Colonia { get; set; }

        [JsonProperty("cp")]
        public string Cp { get; set; }

        [JsonProperty("ciudad")]
        public string Ciudad { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("correo")]
        public string Correo { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }
    }

    public class ResultObjectPerson
    {       
        [JsonProperty("personas")]
        public List<Persona> ListadePersonas { get; set; }
    }
    public partial class Persona
    {
        [JsonProperty("idpersona")]
        public int Idpersona { get; set; }
        [JsonProperty("ape_pat")]
        public string ApePat { get; set; }
        [JsonProperty("ape_mat")]
        public string ApeMat { get; set; }
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
        [JsonProperty("sexo")]
        public string Sexo { get; set; }
        [JsonProperty("fchnac")]
        [NotMapped] public DateTime? Fecha_Nacimiento { get; set; } = DateTime.MinValue;
        [JsonProperty("fecha_nacimiento")]
        public DateTime? Fchnac { get; set; } = DateTime.MinValue;
        [JsonProperty("rfc")]
        public string Rfc { get; set; }
        [JsonProperty("curp")]
        public string Curp { get; set; }
        [JsonProperty("nss")]
        public string Nss { get; set; }
        [JsonProperty("calle")]
        public string Calle { get; set; }
        [JsonProperty("noext")]
        public string Noext { get; set; }
        [JsonProperty("noint")]
        public string Noint { get; set; }
        [JsonProperty("colonia")]
        public string Colonia { get; set; }
        [JsonProperty("cp")]
        public string Cp { get; set; }
        [JsonProperty("ciudad")]
        public string Ciudad { get; set; }
        [JsonProperty("estado")]
        public string Estado { get; set; }
        // public int Edad
        // {
        //     get
        //     {
        //       //  DateTime now = DateTime.Today;
        //         int Edad = 0;
        //         int year = Fchnac == null ? 1990 : Fchnac.Value.Year;

        //Edad = DateTime.Today.Year - year;

        //         if (DateTime.Today < Fchnac.Value.AddYears(Edad))
        //             return --Edad;
        //         else
        //             return Edad;
        //     }
        // }
        [JsonProperty("correo")]
        public string Correo { get; set; }
        [JsonProperty("telefono")]
        public string Telefono { get; set; }
    }
}
