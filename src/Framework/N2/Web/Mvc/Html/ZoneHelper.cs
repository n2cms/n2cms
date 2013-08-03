using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Edit;
using N2.Web.Parts;
using N2.Engine;

namespace N2.Web.Mvc.Html
{
	public class ZoneHelper
	{
		private ContentItem currentItem;
		private PartsAdapter partsAdapter;

		protected System.Web.Mvc.TagBuilder Wrapper { get; set; }

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
			Wrapper = new System.Web.Mvc.TagBuilder(tagName);
			Wrapper.MergeAttributes(new RouteValueDictionary(attributes));
			Wrapper.InnerHtml = innerHtml;

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

        public virtual void Render()
        {
            Render(Html.ViewContext.Writer);
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
			if (Wrapper != null)
			{
				writer.Write(Wrapper.ToString(TagRenderMode.StartTag));
				writer.Write(Wrapper.InnerHtml);
			}
			Adapters.ResolveAdapter<PartsAdapter>(model).RenderPart(Html, model);

			if (Wrapper != null)
                writer.WriteLine(Wrapper.ToString(TagRenderMode.EndTag));
        }
	}
}