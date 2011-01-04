using System;
using System.Security.Principal;

namespace N2.Plugin
{
	/// <summary>
	/// Interface denoting plug-in attributes that are displayed throughout 
	/// the editing interface.
	/// </summary>
    public interface IPlugin : IComparable<IPlugin>
    {
        string Name { get; set; }
        Type Decorates { get; set; }
        int SortOrder { get; }
        bool IsAuthorized(IPrincipal user);
    }
}
