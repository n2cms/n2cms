using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Marks a class or interface as being searchable by type name. Names of interfaces marked with 
    /// this attribute will be stored in the index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    [Obsolete("All types and interfaces are not indexed by default")]
    public sealed class SearchableTypeAttribute : Attribute
    {
    }
}
