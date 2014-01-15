using System;
using N2.Definitions;

namespace N2.Integrity
{
    /// <summary>
    /// Base class for attributes used to restrict which types can be created below which.
    /// </summary>
    public abstract class TypeIntegrityAttribute : AbstractDefinitionRefiner
    {
        private Type[] types = new Type[0];

        /// <summary>Gets or sets the types needed by this attribute.</summary>
        public Type[] Types
        {
            get { return types; }
            set { types = value; }
        }

        /// <summary>Tells wether any of the types defined by the <see cref="Types"/> property are assignable by the supplied type.</summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True of any of the types was assignable from the supplied type.</returns>
        protected virtual bool IsAssignable(Type type)
        {
            if(Types == null)
                return false;

            foreach (Type t in Types)
            {
                if (t.IsAssignableFrom(type))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
