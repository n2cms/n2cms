using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH
{
    internal static class GeneratorHelper
    {
        public static bool IsUnsuiteableForMapping(Type t)
        {
            return t.IsAbstract || t.IsGenericType || string.IsNullOrEmpty(t.FullName);
        }
    }
}
