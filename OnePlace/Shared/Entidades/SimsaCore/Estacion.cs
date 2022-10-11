using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnePlace.Shared.Entidades.SimsaCore
{
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
        public string Fchcreacion { get; set; }
        public string Fchmodificacion { get; set; }
        public int? Idusuario { get; set; }
        public int? Estatus { get; set; }
        public int? Idrazonsocial { get; set; }

        //propiedades de navegacion
        public List<Empleado> Empleados { get; set; }
    }
}
