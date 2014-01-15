using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.IO;

namespace N2.Web.Mvc.Html
{
    /// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
    public class Template<T>
    {
        public T Data { get; set; }
        public Action<TextWriter> ContentRenderer { get; set; }
        static Action<TextWriter> fallback = (tw) => { };

        public HelperResult RenderContents()
        {
            return new HelperResult(ContentRenderer ?? fallback);
        }
    }
}
