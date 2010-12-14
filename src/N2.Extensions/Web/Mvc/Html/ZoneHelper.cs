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

        public ZoneHelper(HtmlHelper helper, string zoneName, ContentItem currentItem)
			: base(helper, currentItem)
        {
            ZoneName = zoneName;
        }

		protected System.Web.Mvc.TagBuilder Wrapper { get; set; }

		protected string ZoneName { get; set; }

		public ZoneHelper WrapIn(string tagName, object attributes)
		{
			Wrapper = new System.Web.Mvc.TagBuilder(tagName);
			Wrapper.MergeAttributes(new RouteValueDictionary(attributes));

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

        public virtual void Render()
        {
            Render(Html.ViewContext.HttpContext.Response.Output);
        }

        public virtual void Render(TextWriter writer)
        {
			foreach (var child in PartsAdapter.GetItemsInZone(CurrentItem, ZoneName))
            {
                RenderTemplate(writer, child);
            }
        }

        protected virtual void RenderTemplate(TextWriter writer, ContentItem model)
        {
            if (Wrapper != null)
                writer.Write(Wrapper.ToString(TagRenderMode.StartTag));

			var adapter = Adapters.ResolveAdapter<MvcAdapter>(model);
			adapter.RenderTemplate(Html, model);

            if (Wrapper != null)
                writer.WriteLine(Wrapper.ToString(TagRenderMode.EndTag));
        }
	}
}