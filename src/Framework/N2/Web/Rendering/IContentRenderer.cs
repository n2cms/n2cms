using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace N2.Web.Rendering
{
    public interface IContentRenderer
    {
        Type HandledContentType { get; }

        void Render(ContentRenderingContext context, TextWriter writer);
    }
}
