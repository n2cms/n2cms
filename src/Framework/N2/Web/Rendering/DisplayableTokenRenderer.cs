using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using N2.Details;
using N2.Engine;

namespace N2.Web.Rendering
{
    [Service(typeof(IDisplayableRenderer))]
    public class DisplayableTokenRenderer : DisplayableRendererBase<DisplayableTokensAttribute>
    {
        public override void Render(RenderingContext context, DisplayableTokensAttribute displayable, TextWriter writer)
        {
            displayable.Render(context, writer);
        }
    }
}
