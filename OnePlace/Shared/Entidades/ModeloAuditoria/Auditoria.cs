using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.Entidades
{
    //la clase se podria decir que es abstracta
    public class Auditoria
    {
        public int Id { get; set; }//id de la tabla auditoria
        public string UserId { get; set; }//id del usuario que modifico
        public string Type { get; set; }//Crear/Eliminar/Actualizar
        public string TableName { get; set; }//Nombre de la tabla
        public DateTime DateTime { get; set; }//fecha
        public string OldValues { get; set; }//valores viejos
        public string NewValues { get; set; }//valores nuevos
        public string AffectedColumns { get; set; }//columnas afectadas
        public string PrimaryKey { get; set; }//id del registro modificado
    }
}
