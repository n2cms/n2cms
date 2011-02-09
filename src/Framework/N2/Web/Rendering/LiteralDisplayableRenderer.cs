using N2.Details;
using N2.Engine;

namespace N2.Web.Rendering
{
	[Service(typeof(IDisplayableRenderer))]
	public class LiteralDisplayableRenderer : DisplayableRendererBase<IWritingDisplayable>
	{
		public override void Render(RenderingContext context, IWritingDisplayable displayable)
		{
			displayable.Write(context.Content, context.PropertyName, context.Writer);
		}
	}
}
