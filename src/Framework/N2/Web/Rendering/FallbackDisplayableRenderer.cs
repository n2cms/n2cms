using System.IO;
using System.Web.UI;
using N2.Details;
using N2.Engine;

namespace N2.Web.Rendering
{
    [Service(typeof(IDisplayableRenderer))]
    public class FallbackDisplayableRenderer : DisplayableRendererBase<IDisplayable>
    {
        public override void Render(RenderingContext context, IDisplayable displayable, TextWriter writer)
        {
            var viewControl = context.Html.ViewContext.View as Control;
            var container = new Control { Page = viewControl != null ? viewControl.Page : null };
            displayable.AddTo(context.Content, context.PropertyName, container);

            container.RenderControl(new HtmlTextWriter(writer));
        }
    }
}
