using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Templates.Wiki.Renderers;
using N2.Plugin;

namespace N2.Templates.Wiki
{
    public class WikiRenderer
    {
        IDictionary<string, IRenderer> map = new Dictionary<string, IRenderer>();

        public WikiRenderer(IPluginFinder pluginFinder)
        {
            Map["UserInfo"] = new UserInfoRenderer();
            Map["InternalLink"] = new InternalLinkRenderer();
            Map["ExternalLink"] = new ExternalLinkRenderer();
            Map["Text"] = FallbackRenderer = new TextRenderer();
            Map["Template"] = new TemplateRenderer(pluginFinder.GetPlugins<ITemplateRenderer>());
        }

        public IRenderer FallbackRenderer { get; set; }

        public IDictionary<string, IRenderer> Map
        {
            get { return map; }
            set { map = value; }
        }

        public void AddTo(IEnumerable<Fragment> fragments, Control container, ContentItem wiki, ContentItem article)
        {

            foreach (Fragment f in fragments)
            {
                var ctx = new RenderingContext { Article = article, Wiki = wiki, Fragment = f };
                if (Map.ContainsKey(f.Name))
                    Map[f.Name].AddTo(container, ctx);
                else if (FallbackRenderer != null)
                    FallbackRenderer.AddTo(container, ctx);
            }
        }
    }
}
