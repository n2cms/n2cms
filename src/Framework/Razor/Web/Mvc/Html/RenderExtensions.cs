using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.WebPages;
using N2.Details;

namespace N2.Web.Mvc.Html
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
    public static class RenderExtensions
    {
        #region class DisplayWrapper
        class DisplayWrapper : IDisplayRenderer, IHtmlString
        {
            public IDisplayRenderer Wrapped { get; set; }
            public Func<Template<IDisplayRenderer>, HelperResult> Template { get; set; }

            #region IDisplayRenderer Members

            public Rendering.RenderingContext Context { get { return Wrapped.Context; } }
            
            public void Render()
            {
                WriteTo(Context.Html.ViewContext.Writer);
            }

            public void WriteTo(System.IO.TextWriter writer)
            {
                if(Template == null)
                {
                    Wrapped.WriteTo(writer);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    Wrapped.WriteTo(sw);

                    if (sb.Length == 0)
                        return;

                    Template(new Template<IDisplayRenderer> { ContentRenderer = (w) => w.Write(sb), Data = Wrapped }).WriteTo(writer);
                }
            }

            public override string ToString()
            {
                using (var sw = new StringWriter())
                {
                    WriteTo(sw);
                    return sw.ToString();
                }
            }

            #endregion

            #region IHtmlString Members

            public string ToHtmlString()
            {
                return ToString();
            }

            #endregion
        }
        #endregion

        public static IDisplayRenderer Wrap(this IDisplayRenderer renderer, Func<Template<IDisplayRenderer>, HelperResult> template)
        {
            return new DisplayWrapper { Wrapped = renderer, Template = template };
        }

        public static IDisplayRenderer Editable(this IDisplayRenderer render, bool isEditable)
        {
            render.Context.IsEditable = isEditable;
            return render;
        }



        public static DisplayRenderer<DisplayableImageAttribute> Image(this RenderHelper render, string detailName)
        {
            return render.Displayable<DisplayableImageAttribute>(detailName);
        }
        public static DisplayRenderer<DisplayableImageAttribute> Image(this RenderHelper render, string detailName, string preferredSize, string cssClass = null)
        {
            return render.Displayable(new DisplayableImageAttribute { Name = detailName, PreferredSize = preferredSize, CssClass = cssClass ?? preferredSize });
        }

        public static DisplayRenderer<DisplayableHeadingAttribute> Heading(this RenderHelper render, string detailName)
        {
            return render.Displayable(new DisplayableHeadingAttribute { Name = detailName });
        }
        public static DisplayRenderer<DisplayableHeadingAttribute> Heading(this RenderHelper render, string detailName, int headingLevel)
        {
            return render.Displayable(new DisplayableHeadingAttribute { Name = detailName, HeadingLevel = headingLevel });
        }

        public static DisplayRenderer<DisplayableLiteralAttribute> Text(this RenderHelper render, string detailName)
        {
            return render.Displayable(new DisplayableLiteralAttribute { Name = detailName });
        }

        public static DisplayRenderer<DisplayableTokensAttribute> Tokens(this RenderHelper render, string detailName)
        {
            return render.Displayable(new DisplayableTokensAttribute { Name = detailName });
        }
    }
}
