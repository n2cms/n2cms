using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using N2.Details;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
    public class RenderHelper
    {
        #region Static
        public static bool DefaultSwallowExceptions = false;
        public static bool DefaultEditable = true;
        public static bool DefaultOptional = true;
        public static DisplayRendererFactory RendererFactory { get; set; }

        static RenderHelper()
        {
            RendererFactory = new DisplayRendererFactory();
        }
        #endregion

        public HtmlHelper Html { get; set; }
        public ContentItem Content { get; set; }

        public DisplayRenderer<T> Displayable<T>(string detailName) where T : IDisplayable, new()
        {
            return Displayable(new T() { Name = detailName });
        }

        public DisplayRenderer<T> Displayable<T>(T displayable) where T : IDisplayable
        {
            return RendererFactory.Create<T>(new Rendering.RenderingContext
            {
                IsEditable = DefaultEditable,
                Content = Content,
                Displayable = displayable,
                Html = Html,
                PropertyName = displayable.Name
            });
        }

        #region class DisplayRendererFactory
        public class DisplayRendererFactory
        {
            public virtual DisplayRenderer<T> Create<T>(Rendering.RenderingContext context) where T : IDisplayable
            {
                return new DisplayRenderer<T>(context);
            }
        }
        #endregion
    }
}
