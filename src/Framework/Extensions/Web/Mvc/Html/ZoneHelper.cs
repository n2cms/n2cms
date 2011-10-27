using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Edit.Workflow;

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
                writer.Write(Wrapper.ToString(TagRenderMode.StartTag));

			var adapter = Adapters.ResolveAdapter<MvcAdapter>(model);
			adapter.RenderTemplate(Html, model);

            if (Wrapper != null)
                writer.WriteLine(Wrapper.ToString(TagRenderMode.EndTag));
        }
	}
}