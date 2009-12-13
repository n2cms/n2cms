using System;
using System.Text;
using System.Web.Routing;
using N2.Collections;
using N2.Web.UI;
using System.Web.Mvc;
using N2.Web.UI.WebControls;
using System.IO;

namespace N2.Web.Mvc.Html
{
	public class ZoneHelper : ItemHelper
	{
		private readonly ITemplateRenderer _templateRenderer = Context.Current.Resolve<ITemplateRenderer>();

        public ZoneHelper(ViewContext viewContext, string zoneName)
            : base(viewContext)
        {
            ZoneName = zoneName;
        }
        public ZoneHelper(ViewContext viewContext, string zoneName, ContentItem currentItem)
            : base(viewContext, currentItem)
        {
            ZoneName = zoneName;
        }

		protected System.Web.Mvc.TagBuilder TagBuilder { get; set; }

		protected string ZoneName { get; set; }

		public ZoneHelper WrapIn(string tagName, object attributes)
		{
			TagBuilder = new System.Web.Mvc.TagBuilder(tagName);
			TagBuilder.MergeAttributes(new RouteValueDictionary(attributes));

			return this;
		}

		public override string ToString()
		{
			var partialResult = new StringBuilder();
            using (var writer = new StringWriter(partialResult))
            {
                Render(writer);
            }
			return partialResult.ToString();
		}

        public virtual void Render(TextWriter writer)
        {
            foreach (var child in GetItems())
            {
                RenderTemplate(writer, child);
            }
        }

        protected virtual void RenderTemplate(TextWriter writer, ContentItem model)
        {
            if (TagBuilder != null)
                writer.Write(TagBuilder.ToString(TagRenderMode.StartTag));

            string partial = _templateRenderer.RenderTemplate(model, ViewContext);
            writer.Write(partial);

            if (TagBuilder != null)
                writer.WriteLine(TagBuilder.ToString(TagRenderMode.EndTag));
        }

		private ItemList GetItems()
		{
			if (PartsAdapter == null)
				return CurrentItem.GetChildren(ZoneName);

			return PartsAdapter.GetItemsInZone(CurrentItem, ZoneName);
		}
	}
}