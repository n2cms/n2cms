using System;
using System.Web;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using System.IO;
using N2.Definitions;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
    public class RegisteringDisplayRenderer<T> : EditableBuilder<T>,
        IHtmlString, 
        IDisplayRenderer where T : IEditable
    {
        public RenderingContext Context { get; set; }

        public RegisteringDisplayRenderer(RenderingContext context, ContentRegistration registration)
            : base(context.PropertyName, registration)
        {
            this.Context = context;
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
