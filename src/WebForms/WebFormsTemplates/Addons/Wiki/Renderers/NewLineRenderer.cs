using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace N2.Addons.Wiki.Renderers
{
    public class NewLineRenderer : IRenderer
    {
        public System.Web.UI.Control AddTo(System.Web.UI.Control container, ViewContext context)
        {
            if (context.Previous == null || context.Next == null)
                return null;
            if (context.Previous.Command != "Text" || context.Next.Command != "Text")
                return null;

            var l = new LiteralControl();
            l.Text = string.Concat(Enumerable.Range(0, context.Fragment.Argument.Count(c => c == '\n')).Select(i => "<br/>"));
            container.Controls.Add(l);
            return l;
               
        }
    }
}
