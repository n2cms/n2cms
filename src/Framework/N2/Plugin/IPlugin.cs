using System;
using N2.Security;

namespace N2.Plugin
{
    /// <summary>
    /// Interface denoting plug-in attributes that are displayed throughout 
    /// the editing interface.
    /// </summary>
    public interface IPlugin : IComparable<IPlugin>, ISecurableBase
    {
        string Name { get; set; }
        Type Decorates { get; set; }
        int SortOrder { get; }
    }
}
