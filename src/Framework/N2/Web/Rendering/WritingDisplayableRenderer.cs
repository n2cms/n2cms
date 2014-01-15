using System.IO;
using N2.Details;
using N2.Engine;

namespace N2.Web.Rendering
{
    [Service(typeof(IDisplayableRenderer))]
    public class WritingDisplayableRenderer : DisplayableRendererBase<IWritingDisplayable>
    {
        public override void Render(RenderingContext context, IWritingDisplayable displayable, TextWriter writer)
        {
            displayable.Write(context.Content, context.PropertyName, writer);
        }
    }
}
