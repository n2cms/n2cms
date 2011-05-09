using System.Web.Mvc;
using N2.Engine;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	[Adapts(typeof(ContentItem))]
	public class MvcAdapter : AbstractContentAdapter
	{
		ITemplateRenderer renderer;

		public ITemplateRenderer Renderer
		{
			get { return renderer ?? (renderer = Engine.Resolve<ITemplateRenderer>()); }
			set { renderer = value; }
		}

		public virtual void RenderTemplate(HtmlHelper html, ContentItem model)
		{
			Renderer.RenderTemplate(model, html);
		}
	}
}
