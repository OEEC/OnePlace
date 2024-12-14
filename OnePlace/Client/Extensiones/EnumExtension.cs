using System;
using System.ComponentModel;
using System.Reflection;

namespace OnePlace.Client.Extenciones
{
    public static class EnumExtension
    {
        public static string Descripcion(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);

            if (!string.IsNullOrEmpty(name))
            {
                FieldInfo field = type.GetField(name);
                if (field is not null)
                {
                    DescriptionAttribute description = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (description is not null)
                    {
                        return description.Description;
                    }
                }
            }

            return name;
        }
    }
}
