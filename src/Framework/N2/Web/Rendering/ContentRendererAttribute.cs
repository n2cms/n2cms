using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Web.Rendering
{
    /// <summary>
    /// Marks a class implementing <see cref="IContentRenderer"/> or <see cref="ContentRendererBase"/>
    /// as a service which is resolved when rendering content items.
    /// </summary>
    public class ContentRendererAttribute : ServiceAttribute
    {
        public ContentRendererAttribute()
            : base(typeof(IContentRenderer))
        {
        }
    }
}
