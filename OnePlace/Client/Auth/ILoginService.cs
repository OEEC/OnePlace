using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Auth
{
    public interface ILoginService
    {
        //creamos dos asignaturas de metodos
        //login va a recibir un token y se va encargar de guardar el token en localstorage
        Task Login(string token);

        //logout se va encargar de eliminar de localstorage el token
        Task Logout();
    }
}
