using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class UsuarioDTO
    {
        public string UserId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Contraseña { get; set; }//se puso para poder actualizar la contraseña en texto plano de la tabla aspnetuser
        public int CodigoEmpleado { get; set; }//se puso para filtrar los usuarios por empleado
        public bool Activo { get; set; }//se puso para cambiar el estado y no borrar de la bd el usuario
        public string Estacion { get; set; }//se puso para mostrar en el listado de usuarios a que estacion pertenece
    }
}
