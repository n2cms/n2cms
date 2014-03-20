using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Addons.Wiki.Renderers;
using N2.Plugin;
using N2.Web;
using N2.Engine;
using N2.Web.Parsing;

namespace N2.Addons.Wiki
{
    /// <summary>
    /// Turns a stream of wiki fragments into asp.net controls responsible for 
    /// rendering the user interface.
    /// </summary>
    [Service]
    public class WikiRenderer
    {
        IDictionary<string, IRenderer> renderers = new Dictionary<string, IRenderer>();

        public WikiRenderer(IPluginFinder pluginFinder, IWebContext webContext)
        {
            Renderers["Comment"] = new CommentRenderer();
            Renderers["UserInfo"] = new UserInfoRenderer();
            Renderers["InternalLink"] = new InternalLinkRenderer(webContext);
            Renderers["ExternalLink"] = new ExternalLinkRenderer();
            Renderers["Text"] = FallbackRenderer = new TextRenderer();
            Renderers["Template"] = new TemplateRenderer(pluginFinder.GetPlugins<ITemplateRenderer>());
            Renderers["Heading"] = new HeadingRenderer();
            Renderers["Line"] = new LineRenderer();
            Renderers["OrderedListItem"] = new OrderedListItemRenderer();
            Renderers["UnorderedListItem"] = new UnorderedListItemRenderer();
            Renderers["Bold"] = new FormatRenderer();
            Renderers["Italics"] = new FormatRenderer();
            Renderers["BoldItalics"] = new FormatRenderer();
            Renderers["NewLine"] = new NewLineRenderer();
        }

        public IRenderer FallbackRenderer { get; set; }

        public IDictionary<string, IRenderer> Renderers
        {
            get { return renderers; }
        }

        public void AddTo(IEnumerable<Component> fragments, Control container, IArticle article)
        {
            if (fragments == null) throw new ArgumentNullException("fragments");
            if (container == null) throw new ArgumentNullException("container");
            if (article == null) throw new ArgumentNullException("article");

            AddTo(fragments, container, article, new Dictionary<string, object>());
        }

        protected virtual void AddTo(IEnumerable<Component> fragments, Control container, IArticle article, IDictionary<string, object> state)
        {
            var list = fragments.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var f = list[i];
                var ctx = new ViewContext { Renderer = this, Article = article, Fragment = f, State = state };
                if (i > 0)
                    ctx.Previous = list[i - 1];
                if (i < list.Count - 1)
                    ctx.Next = list[i + 1];

                if (Renderers.ContainsKey(f.Command))
                {
                    Control c = Renderers[f.Command].AddTo(container, ctx);
                }
                else if (FallbackRenderer != null)
                {
                    FallbackRenderer.AddTo(container, ctx);
                }
            }
        }
    }
}
