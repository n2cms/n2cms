using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Templates.Wiki.Renderers;
using N2.Plugin;
using N2.Web;

namespace N2.Templates.Wiki
{
    /// <summary>
    /// Turns a stream of wiki fragments into asp.net controls responsible for 
    /// rendering the user interface.
    /// </summary>
    public class WikiRenderer
    {
        IDictionary<string, IRenderer> map = new Dictionary<string, IRenderer>();

        public WikiRenderer(IPluginFinder pluginFinder, IWebContext webContext)
        {
            Map["Comment"] = new CommentRenderer();
            Map["UserInfo"] = new UserInfoRenderer();
            Map["InternalLink"] = new InternalLinkRenderer(webContext);
            Map["ExternalLink"] = new ExternalLinkRenderer();
            Map["Text"] = FallbackRenderer = new TextRenderer();
            Map["Template"] = new TemplateRenderer(pluginFinder.GetPlugins<ITemplateRenderer>());
            Map["Heading"] = new HeadingRenderer();
            Map["Line"] = new LineRenderer();
            Map["OrderedList"] = Map["UnorderedList"] = new ListRenderer();
            Map["Format"] = new FormatRenderer();
        }

        public IRenderer FallbackRenderer { get; set; }

        public IDictionary<string, IRenderer> Map
        {
            get { return map; }
            set { map = value; }
        }

        public void AddTo(IEnumerable<Fragment> fragments, Control container, IWiki wiki, ContentItem article)
        {
            foreach (Fragment f in fragments)
            {
                var ctx = new ViewContext { Article = article as IArticle, Fragment = f };
                if (Map.ContainsKey(f.Name))
                    Map[f.Name].AddTo(container, ctx);
                else if (FallbackRenderer != null)
                    FallbackRenderer.AddTo(container, ctx);
            }
        }
    }
}
