using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Edit;
using N2.Web.Parts;
using N2.Engine;
using System;
using System.Web;

namespace N2.Web.Mvc.Html
{
    public class ZoneHelper : IHtmlString
    {
        private ContentItem currentItem;
        private PartsAdapter partsAdapter;

        protected Func<ContentItem, TagBuilder> Wrapper { get; set; }

        protected string ZoneName { get; set; }

        public HtmlHelper Html { get; set; }

        protected ContentItem CurrentItem
        {
            get { return currentItem ?? (currentItem = Html.CurrentItem()); }
            set { this.currentItem = value; }
        }

        protected IContentAdapterProvider Adapters
        {
            get { return Html.ResolveService<IContentAdapterProvider>(); }
        }

        /// <summary>The content adapter related to the current page item.</summary>
        protected virtual PartsAdapter PartsAdapter
        {
            get
            {
                if (partsAdapter == null)
                    partsAdapter = Adapters.ResolveAdapter<PartsAdapter>(CurrentItem);
                return partsAdapter;
            }
        }

        public ZoneHelper(HtmlHelper helper, string zoneName, ContentItem currentItem)
        {
            Html = helper;
            CurrentItem = currentItem;
            ZoneName = zoneName;
        }

        public ZoneHelper WrapIn(string tagName, object attributes, string innerHtml = null)
        {
            Wrapper = (ci) =>
            {
                var w = new System.Web.Mvc.TagBuilder(tagName);
                w.MergeAttributes(new RouteValueDictionary(attributes));
                w.InnerHtml = innerHtml;
                return w;
            };

            return this;
        }

        public ZoneHelper WrapIn(Func<ContentItem, TagBuilder> wrapperFactory)
        {
            Wrapper = wrapperFactory;

            return this;
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                Render(writer);
                return writer.ToString();
            }
        }

        public virtual string Render()
        {
            Render(Html.ViewContext.Writer);
			return "";
        }

        public virtual void Render(TextWriter writer)
        {
            if (N2.Web.Mvc.Html.RegistrationExtensions.GetRegistrationExpression(Html) != null)
                return;

            foreach (var child in PartsAdapter.GetParts(CurrentItem, ZoneName, GetInterface()))
            {
                RenderTemplate(writer, child);
            }
        }

        protected virtual string GetInterface()
        {
            return Interfaces.Viewing;
        }

        protected virtual void RenderTemplate(TextWriter writer, ContentItem model)
        {
            var w = Wrapper != null ? Wrapper(model) : null;
            if (w != null)
            {
                writer.Write(w.ToString(TagRenderMode.StartTag));
                writer.Write(w.InnerHtml);
            }
            Adapters.ResolveAdapter<PartsAdapter>(model).RenderPart(Html, model, writer);

            if (w != null)
                writer.WriteLine(w.ToString(TagRenderMode.EndTag));
        }

		string IHtmlString.ToHtmlString()
		{
			return ToString();
		}
	}
}
