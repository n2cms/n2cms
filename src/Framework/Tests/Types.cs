using System;
using System.Collections.Generic;
using System.Linq;

namespace N2.Tests
{
    public static class Types
    {
        public static IEnumerable<Type> NearbyTypes(this object obj)
        {
            return obj.GetType().Assembly.GetTypes();
        }

        public static IEnumerable<Type> InNamespace(this IEnumerable<Type> types, string @namespace)
        {
            return types.BelowNamespace(@namespace)
                .Where(t => t.Namespace == @namespace);
        }
        public static IEnumerable<Type> BelowNamespace(this IEnumerable<Type> types, string @namespace)
        {
            //var a1 = Assembly.GetCallingAssembly();
            return types
                .Where(t => t != null)
                .Where(t => t.Namespace != null)
                .Where(t => t.Namespace.StartsWith(@namespace))
                .Distinct();
        }
        public static IEnumerable<Type> BelowNamespace<T>(this IEnumerable<Type> types, string @namespace)
        {
            return types.BelowNamespace(@namespace)
                .AssignableTo<T>();
        }

        public static IEnumerable<Type> AssignableTo<T>(this IEnumerable<Type> types)
        {
            return types.Where(t => typeof(T).IsAssignableFrom(t));
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> types, T additional)
        {
            return types.Union((IEnumerable<T>)new[] { additional });
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> types, T removed)
        {
            return types.Except((IEnumerable<T>)new[] { removed });
        }
    }
}
