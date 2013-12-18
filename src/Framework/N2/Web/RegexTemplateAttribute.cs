using System;
using System.Text.RegularExpressions;

namespace N2.Web
{
    /// <summary>
    /// Registers a tempalte to serve a certain content based on regular 
    /// expression match on the remaining path.
    /// </summary>
    /// <example>
    /// // Would map /path/to/my/content/anything/that/matches/the/expression.aspx to AnyPath aspx.
    /// [RegexTemplate(".*", "~/Templates/AnyPath.aspx")]
    /// public class MyContent : ContentItem { }
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RegexTemplateAttribute : Attribute, IPathFinder
    {
        readonly string action;
        readonly Regex expression;
        readonly string templateUrl;

        public RegexTemplateAttribute(string pattern, string templateUrl, string action)
            :this(pattern,templateUrl)
        {
            this.action = action;
        }

        public RegexTemplateAttribute(string pattern, string templateUrl)
        {
            expression = new Regex(pattern);
            this.templateUrl = templateUrl;
        }

        public PathData GetPath(ContentItem item, string remainingUrl)
        {
            if (remainingUrl.EndsWith(item.Extension) && item.Extension.Length > 0)
                remainingUrl = remainingUrl.Substring(0, remainingUrl.Length - item.Extension.Length);

            Match m = expression.Match(remainingUrl);
            if(m.Success)
            {
                return new PathData(item, templateUrl, action, remainingUrl);
            }
            return null;
        }
    }
}
