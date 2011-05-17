using System.Web.Mvc;
using System.Linq;
using N2.Details;
using N2.Web.Mvc.Html;
using N2.Definitions;

namespace N2.Web.Rendering
{
	public class RenderingContext
	{
		public HtmlHelper Html { get; set; }
		public ContentItem Content { get; set; }
		public string PropertyName { get; set; }
		public IDisplayable Displayable { get; set; }

		public static RenderingContext Create(HtmlHelper html, string propertyName)
		{
			var context = new RenderingContext();
			context.Content = html.CurrentItem();
			var template = html.ResolveService<IDefinitionManager>().GetTemplate(context.Content);
			if (template != null)
				context.Displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == propertyName);
			context.Html = html;
			context.PropertyName = propertyName;
			return context;
		}
	}
}
