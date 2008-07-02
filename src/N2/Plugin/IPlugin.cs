using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace N2.Plugin
{
    public interface IPlugin : IComparable<IPlugin>
    {
        string Name { get; set; }
        int SortOrder { get; }
        bool IsAuthorized(IPrincipal user);
    }
}
