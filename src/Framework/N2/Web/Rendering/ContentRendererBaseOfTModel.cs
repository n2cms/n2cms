using System;
using System.IO;

namespace N2.Web.Rendering
{
    public abstract class ContentRendererBase<TModel> : IContentRenderer
    {
        public Type HandledContentType
        {
            get { return typeof(TModel); }
        }

        public abstract void Render(ContentRenderingContext context, TextWriter writer);
    }

}
