using System;
using System.IO;
using N2.Web.Rendering;

namespace N2.Web.Mvc
{
    public interface IDisplayRenderer
    {
        RenderingContext Context { get; }

        void Render();
        string ToString();
        void WriteTo(TextWriter writer);
    }
}
