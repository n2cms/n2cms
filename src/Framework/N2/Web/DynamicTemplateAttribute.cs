using System;

namespace N2.Web
{
    /// <summary>
    /// Register a template reference that uses the content item's 
    /// TemplateUrl property as template.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DynamicTemplateAttribute : Attribute, IPathFinder
    {
        public PathData GetPath(ContentItem item, string remainingUrl)
        {
            if(string.IsNullOrEmpty(remainingUrl))
                return new PathData(item, item.TemplateUrl);
            return null;
        }
    }
}
