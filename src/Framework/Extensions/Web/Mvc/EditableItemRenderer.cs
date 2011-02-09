using N2.Details;
using N2.Engine;
using N2.Web.Mvc;

namespace N2.Web.Rendering
{
	[Service(typeof(IDisplayableRenderer))]
	public class EditableItemRenderer : DisplayableRendererBase<EditableItemAttribute>
	{
		IContentAdapterProvider adapters;

		public EditableItemRenderer(IContentAdapterProvider adapters)
		{
			this.adapters = adapters;
		}

		public override void Render(RenderingContext context, EditableItemAttribute displayable)
		{
			var backup = context.Html.ViewContext.Writer;
			try
			{
				context.Html.ViewContext.Writer = context.Writer;
				var referencedItem = context.Content[context.PropertyName] as ContentItem;
				if (referencedItem != null)
					adapters.ResolveAdapter<MvcAdapter>(context.Content).RenderTemplate(context.Html, referencedItem);
			}
			finally
			{
				context.Html.ViewContext.Writer = backup;
			}
		}
	}
}
