using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Web.Mvc.Html
{
    /// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
    public class ListTemplate<T> : Template<T>
    {
        public int Index { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}
