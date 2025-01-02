using System;
using System.Runtime.Serialization;

namespace OnePlace.Server.Excepciones
{
    public class UsuarioException : Exception
    {
        public UsuarioException()
        {
        }

        public UsuarioException(string message) : base(message)
        {
        }

        public UsuarioException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UsuarioException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
