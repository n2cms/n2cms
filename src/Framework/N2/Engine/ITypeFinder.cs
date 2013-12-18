using System;
using System.Collections.Generic;
using System.Reflection;

namespace N2.Engine
{
    /// <summary>
    /// Classes implementing this interface provide information about types 
    /// to various services in the N2 engine.
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>Finds types assignable from of a certain type in the app domain.</summary>
        /// <param name="requestedType">The type to find.</param>
        /// <returns>A list of types found in the app domain.</returns>
        IEnumerable<Type> Find(Type requestedType);

        /// <summary>Finds types assignable from of a certain type and with a certain attribute in the app domain.</summary>
        /// <param name="requestedType">The type to find.</param>
        /// <param name="inherit"></param>
        /// <typeparam name="TAttribute">The type of attribute the type should be decorated with.</typeparam>
        /// <returns>A list of types found in the app domain.</returns>
        IEnumerable<AttributedType<TAttribute>> Find<TAttribute>(Type requestedType, bool inherit = false) where TAttribute : class;

        /// <summary>Gets tne assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the N2 factory.</returns>
        IEnumerable<Assembly> GetAssemblies();
    }
}
