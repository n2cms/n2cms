using System.Collections.Generic;
using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// The configured collection of paths instructs N2's url rewriter to 
    /// ignore certain virtual paths when considering a path for rewrite.
    /// </summary>
    [ConfigurationCollection(typeof(VirtualPathElement))]
    public class VirtualPathCollection : LazyRemovableCollection<VirtualPathElement>
    {
        public VirtualPathCollection()
        {
            AddDefault(new VirtualPathElement("management", "{ManagementUrl}/"));
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new VirtualPathElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((VirtualPathElement) element).Name;
        }

        public string[] GetPaths(N2.Web.IWebContext webContext)
        {
            List<string> paths = new List<string>();
            foreach (var vpe in AllElements)
            {
                paths.Add(N2.Web.Url.ToAbsolute(N2.Web.Url.ResolveTokens(vpe.VirtualPath)));
            }

            return paths.ToArray();
        }
    }
}
