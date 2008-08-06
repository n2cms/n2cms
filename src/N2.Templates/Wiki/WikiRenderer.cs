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
            Renderers["OrderedList"] = new OrderedListRenderer();
            Renderers["UnorderedList"] = new UnorderedListRenderer();
            Renderers["Format"] = new FormatRenderer();
        }

        public IRenderer FallbackRenderer { get; set; }

        public IDictionary<string, IRenderer> Renderers
        {
            get { return renderers; }
        }

        public void AddTo(IEnumerable<Fragment> fragments, Control container, ContentItem article)
        {
            if (fragments == null) throw new ArgumentNullException("fragments");
            if (container == null) throw new ArgumentNullException("container");
            if (article == null) throw new ArgumentNullException("article");

            AddTo(fragments, container, article, new Dictionary<string, object>());
        }

        protected virtual void AddTo(IEnumerable<Fragment> fragments, Control container, ContentItem article, IDictionary<string,object> state)
        {
            foreach (Fragment f in fragments)
            {
                var ctx = new ViewContext { Article = article as IArticle, Fragment = f, State = state };
                if (Renderers.ContainsKey(f.Name))
                {
                    Control c = Renderers[f.Name].AddTo(container, ctx);
                    if (f.ChildFragments != null)
                        AddTo(f.ChildFragments, c, article, state);
                }
                else if (FallbackRenderer != null)
                {
                    FallbackRenderer.AddTo(container, ctx);
                }
            }
        }
    }
}
