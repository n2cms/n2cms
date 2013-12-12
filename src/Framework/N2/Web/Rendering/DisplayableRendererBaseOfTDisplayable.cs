using System;
using System.IO;

namespace N2.Web.Rendering
{
    public abstract class DisplayableRendererBase<TDisplayable> : IDisplayableRenderer
    {
        public abstract void Render(RenderingContext context, TDisplayable displayable, TextWriter writer);

        #region IDisplayableRenderer Members

        public Type HandledDisplayableType
        {
            get { return typeof(TDisplayable); }
        }

        public void Render(RenderingContext context, TextWriter writer)
        {
            Render(context, (TDisplayable)context.Displayable, writer);
        }

        #endregion
    }

}
