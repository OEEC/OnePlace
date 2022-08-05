using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.Repositorios
{
    public interface IRepositorio
    {
        Task<HttpResponseWrapper<object>> Delete(string url);
        Task<HttpResponseWrapper<T>> Get<T>(string url); 
        Task<HttpResponseWrapper<object>> Post<T>(string url, T enviar);//generamos una asignatura que hace referencia al metodo post de repositorio.cs 
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar);
        Task<HttpResponseWrapper<object>> Put<T>(string url, T enviar);
    }
}
