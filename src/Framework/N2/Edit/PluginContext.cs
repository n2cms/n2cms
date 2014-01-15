using System;
using System.Text.RegularExpressions;
using System.Web;
using N2.Engine;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// Class used to supply plugins with contextual information.
    /// </summary>
    public class PluginContext
    {
        public PluginContext(SelectionUtility selection, ContentItem startItem, ContentItem rootItem, ControlPanelState state, 
            IEngine engine, HttpContextBase httpContext)
        {
            Selection = selection;
            State = state;
            Selected = selection.SelectedItem;
            Memorized = selection.MemorizedItem;
            Start = startItem;
            Root = rootItem;

            Engine = engine;
            HttpContext = httpContext;
        }

        public SelectionUtility Selection { get; set; }
        public ControlPanelState State { get; set;}
        public ContentItem Selected { get; set; }
        public ContentItem Memorized { get; set; }
        public ContentItem Start { get; set; }
        public ContentItem Root { get; set; }
        public HttpContextBase HttpContext { get; set; }
        public IEngine Engine { get; set; }
        
        static readonly Regex expressionExpression = new Regex("{(?<expr>[^})]+)}");

        public string Format(string format, bool urlEncode)
        {
            format = Url.ResolveTokens(format).Replace("{selected}", "{Selected.Path}")
                .Replace("{memory}", "{Memorized.Path}")
                .Replace("{action}", "{Action}");

            return expressionExpression.Replace(format, delegate(Match m) { return Evaluate(m.Groups["expr"].Value, urlEncode); });
        }

        public string Rebase(string url)
        {
            if (String.IsNullOrEmpty(url))
                url = "empty.aspx";

            string rebasedUrl = Engine.ManagementPaths.ResolveResourceUrl(url);
            return rebasedUrl;
        }

        string Evaluate(string expression, bool urlEncode)
        {
            object value = Utility.Evaluate(this, expression);
            
            if (value == null)
                return null;

            if(urlEncode)
                return HttpUtility.UrlEncode(value.ToString());

            return value.ToString();
        }
    }
}
