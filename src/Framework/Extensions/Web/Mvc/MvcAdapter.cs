using System.Web.Mvc;
using N2.Engine;
using N2.Web.Mvc.Html;
using System;

namespace N2.Web.Mvc
{
	[Adapts(typeof(ContentItem))]
	public class MvcAdapter : AbstractContentAdapter
	{
		ITemplateRenderer renderer;
		Rendering.ContentRendererSelector rendererSelector;

		public ITemplateRenderer Renderer
		{
			get { return renderer ?? (renderer = Engine.Resolve<ITemplateRenderer>()); }
			set { renderer = value; }
		}

		public Rendering.ContentRendererSelector RendererSelector
		{
			get { return rendererSelector ?? (rendererSelector = Engine.Resolve<Rendering.ContentRendererSelector>()); }
			set { rendererSelector = value; }
		}

//#pragma warning disable 618
//#pragma warning restore 618

		//[Obsolete("This method for overriding rendering of parts is deprecated in favor for the content renderer system. Try implementing IContentRenderer or adding a class [ContentRenderer] public class MyRenderer : ContentRendererBase<My>")]
		public virtual void RenderTemplate(HtmlHelper html, ContentItem model)
		{
			var renderer = model as Rendering.IContentRenderer
				?? RendererSelector.ResolveRenderer(model.GetContentType());
			if (renderer != null)
			{
				renderer.Render(new Rendering.ContentRenderingContext { Content = model, Html = html }, html.ViewContext.Writer);
				return;
			}

			Renderer.RenderTemplate(model, html);
		}
	}
}
