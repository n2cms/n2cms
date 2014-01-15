using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
    /// <summary>
    /// Incstructs the system not to rewrite to the template of the given item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NonRewritableAttribute : Attribute, IPathFinder
    {
        public PathData GetPath(ContentItem item, string remainingUrl)
        {
            if (string.IsNullOrEmpty(remainingUrl))
                return new PathData(item) { IsRewritable = false };
                
            return null;
        }
    }
}
