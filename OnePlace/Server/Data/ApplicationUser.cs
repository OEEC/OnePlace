using Microsoft.AspNetCore.Identity;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Server.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string noemp { get; set; }
        public bool Activo { get; set; }
        public string ContraseñaTextoPlano { get; set; }//poder ver la contraseña sin hash

        //propiedades de navegacion
        public int Idempleado { get; set; }//el nombre debe de ser igual al campo en la bd
        //public virtual Empleado Empleado { get; set; } // si ponemos el virtual la bd no lo reconoce por que no esta fisicamente en algun lugar esto lo hacia EF por "detras de camaras"
               
        //este valor no lo acepta mysql, se tiene que mapear como string
        //public TipodeUsuario TipodeUsuarios { get; set; }
        public string TipodeUsuarios { get; set; }
        public enum TipodeUsuario
        {
            Usuario = 1,//Usuario del sistema, tiene menos privilegios que el administrador
            Administrador//Administrador del sistema tiene todos los privilegios y permisos                      
        }
    }   
}
