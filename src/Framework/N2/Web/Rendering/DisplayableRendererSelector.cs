using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using N2.Engine;
using System.Web.Routing;

namespace N2.Web.Rendering
{
	[Service]
	public class DisplayableRendererSelector
	{
		IDisplayableRenderer[] renderers;
		Dictionary<Type, IDisplayableRenderer> rendererForType = new Dictionary<Type, IDisplayableRenderer>();

		public DisplayableRendererSelector(IDisplayableRenderer[] renderers)
		{
			this.renderers = renderers
				.OrderByDescending(r => Utility.InheritanceDepth(r.HandledDisplayableType))
				.ThenByDescending(r => Utility.InheritanceDepth(r.GetType()))
				.ToArray();
		}

		public virtual IDisplayableRenderer ResolveRenderer(Type displayableType)
		{
			IDisplayableRenderer renderer;
			if (rendererForType.TryGetValue(displayableType, out renderer))
				return renderer;

			var temp = new Dictionary<Type, IDisplayableRenderer>(rendererForType);
			temp[displayableType] = renderer = renderers.FirstOrDefault(r => r.HandledDisplayableType.IsAssignableFrom(displayableType));
			rendererForType = temp;

			return renderer;
		}

		public void Render(RenderingContext context, TextWriter writer)
		{
			var tw = context.IsEditable 
				? TagWrapper.Begin("div", writer, htmlAttributes: new RouteValueDictionary { { "data-id", context.Content.ID }, { "data-path", context.Content.Path }, { "data-property", context.PropertyName }, { "data-displayable", context.Displayable.GetType().Name }, { "class", "editable" } })
				: new EmptyDisposable();

			using (tw)
			{
				var renderer = ResolveRenderer(context.Displayable.GetType());
				renderer.Render(context, writer);
			}
		}
	}
}
