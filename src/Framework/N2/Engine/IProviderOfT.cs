using System.Collections.Generic;

namespace N2.Engine
{
    /// <summary>
    /// Provides something. What this is depends on the circumstance.
    /// </summary>
    /// <typeparam name="T">The type of instance that is provided.</typeparam>
    public interface IProvider<T> where T : class
    {
        /// <summary>Gets the default provided object.</summary>
        /// <returns>An object or null if no object can be provided.</returns>
        T Get();

        /// <summary>Gets all instances of the object.</summary>
        /// <returns>All provided objects of this type.</returns>
        IEnumerable<T> GetAll();
    }
}
