using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public DateTime? Fchnac { get; set; }

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
        public List<Person> ListadePersonas { get; set; }
    }
    public partial class Persona
    {
        public int Idpersona { get; set; }
        public string ApePat { get; set; }
        public string ApeMat { get; set; }
        public string Nombre { get; set; }
        public string Sexo { get; set; }   
        public DateTime Fchnac { get; set; }
        public string Rfc { get; set; }
        public string Curp { get; set; }
        public string Nss { get; set; }
        public string Calle { get; set; }
        public string Noext { get; set; }
        public string Noint { get; set; }
        public string Colonia { get; set; }
        public string Cp { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public int Edad
        {
            get
            {
                DateTime now = DateTime.Today;
                int Edad = 0;

                Edad = DateTime.Today.Year - Fchnac.Year;

                if (DateTime.Today < Fchnac.AddYears(Edad))
                    return --Edad;
                else
                    return Edad;
            }
        }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
}
