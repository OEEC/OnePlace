using OnePlace.Shared.Entidades;
using OnePlace.Shared.Entidades.SimsaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class EmpleadoPersonaDTO
    {
        public Empleado Empleado { get; set; }
        public Persona Persona { get; set; }
        public Curso Curso { get; set; }// esta propiedad solo se usa en el certificado
        public DateTime ProximoCumple { get; set; }
        public DateTime ProximoCumpleTodoMes { get; set; }

        //public int Edad { 
        //    get {
        //        DateTime now = DateTime.Today;
        //        int Edad = 0;
        //        int fecha = 0;
        //        if (Persona != null) {
        //           fecha = Persona.Fchnac.Value.Year;
        //        } else {
        //            fecha = 1990;
        //        }
        //       // int fecha = !Persona.Fchnac.HasValue ? 1990 : Persona.Fchnac.Value?.Year;

        //        Edad = DateTime.Today.Year - fecha;
        //       // Edad = DateTime.Today.Year;
        //        if (DateTime.Today < Persona.Fchnac.Value.AddYears(Edad))
        //            return --Edad;
        //        else
        //            return Edad;
        //    } 
        //}
        public string CodigoQR { get; set; }
    }
}
