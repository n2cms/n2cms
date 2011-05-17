using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Web.Mvc.Html;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using N2.Details;

namespace N2.Web.Mvc
{
	public class DisplayRenderer<T> : 
#if NET4
		IHtmlString, 
#endif
		IDisplayRenderer where T : IDisplayable
	{
		public RenderingContext Context { get; set; }

		public DisplayRenderer(RenderingContext context)
		{
			this.Context = context;
		}

		public DisplayRenderer(HtmlHelper html, string propertyName)
		{
			Context = new RenderingContext();
			Context.Content = html.CurrentItem();
			var template = html.ResolveService<IDefinitionManager>().GetTemplate(Context.Content);
			if (template != null)
				Context.Displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == propertyName);
			Context.Html = html;
			Context.PropertyName = propertyName;
		}

		#region IHtmlString Members

		public string ToHtmlString()
		{
			return ToString();
		}

		#endregion

		public void Render()
		{
			if (Context.Displayable == null)
				return;

			WriteTo(Context.Html.ViewContext.Writer);
		}

		public override string ToString()
		{
			using (var sw = new StringWriter())
			{
				WriteTo(sw);
				return sw.ToString();
			}
		}

		public void WriteTo(TextWriter writer)
		{
			if (Context.Displayable == null)
				return;

			Context.Html.ResolveService<DisplayableRendererSelector>()
				.Render(Context, writer);
		}
	}
}