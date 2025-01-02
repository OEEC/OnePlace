using System;

namespace OnePlace.Shared.Extensiones
{
    public static class StringExtension
    {
        public static int ToInt(this string value) => int.TryParse(value, out int result) ? result : throw new ArgumentException("No se puede convertir el parametro a numero");
    }
}
