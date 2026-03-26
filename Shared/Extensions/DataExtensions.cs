using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public static class DataExtensions
    {
        public static void SetAllPropertiesToNullExcept<T>(T obj, string exceptPropertyName)
        {
            if (obj == null) return;

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CanWrite && p.PropertyType.IsClass && p.Name != exceptPropertyName);

            foreach (var prop in properties)
            {
                prop.SetValue(obj, null);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable != null)
            {
                return !enumerable.Any();
            }

            return true;
        }
        //public static bool NotEmpty(this string? value)
        //{
        //    return !string.IsNullOrWhiteSpace(value);
        //}

        //public static bool Positive(this int value)
        //{
        //    return value >= 0;
        //}
    }
}
