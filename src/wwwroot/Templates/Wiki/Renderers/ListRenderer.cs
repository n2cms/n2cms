using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Wiki.Renderers
{
    public abstract class ListRenderer : IRenderer
    {
        protected abstract string BeginTag { get; }
        protected abstract string EndTag { get; }

        public Control AddTo(Control container, ViewContext context)
        {
            Fragment f = context.Fragment;
            int level = f.Value.Length;

            if (f.Previous == null || f.Previous.Name != f.Name || f.Previous.Value.Length != level)
            {
                container.Controls.Add(new LiteralControl(BeginTag));
            }

            HtmlGenericControl li = new HtmlGenericControl("li");
            container.Controls.Add(li);

            if (f.Next == null || f.Next.Name != f.Name || f.Next.Value.Length != level)
            {
                container.Controls.Add(new LiteralControl(EndTag));
            }

            return li;
        }
    }
}
