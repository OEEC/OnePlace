using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class AuditoriaDTO
    {
        public string Nombre { get; set; }//usuario que modifico     
        public string Type { get; set; }//Crear/Eliminar/Actualizar
        public string TableName { get; set; }//Nombre de la tabla
        public DateTime DateTime { get; set; }//fecha
        public Dictionary<string, string> OldValues { get; set; }//valores viejos
        public Dictionary<string, string> NewValues { get; set; }//valores nuevos
        public string AffectedColumns { get; set; }//columnas afectadas       
    }
}
