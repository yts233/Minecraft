using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Extensions
{
    public static class ObjectExtension
    {
        public static string GetPropertyInfoString(this object obj)
        {
            if (obj == null)
                return "null";
            var @string = new StringBuilder();
            var type = obj.GetType();
            @string.Append(type.FullName);
            @string.Append(':');
            foreach (var property in type.GetProperties())
            {
                @string.AppendLine();
                @string.Append(property.Name);
                @string.Append(": ");
                @string.Append(property.GetValue(obj)?.ToString() ?? null);
            }
            return @string.ToString();
        }

        public static Logger<T> GetLogger<T>(this T _)
        {
            return Logger.GetLogger<T>();
        }
    }
}
