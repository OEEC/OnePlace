using System;
using System.Collections.Generic;

namespace OnePlace.Shared.Entidades.SimsaCore
{
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
