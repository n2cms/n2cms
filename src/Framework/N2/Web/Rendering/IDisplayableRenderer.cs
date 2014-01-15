using System;
using System.IO;

namespace N2.Web.Rendering
{
    public interface IDisplayableRenderer
    {
        Type HandledDisplayableType { get; }

        void Render(RenderingContext context, TextWriter writer);
    }

}
