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
        public string CodigoQR { get; set; }
    }
}
