using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Addons.Wiki.Renderers
{
    public class FormatRenderer : IRenderer
    {
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string text = context.Fragment.ToString().TrimStart('\'');
            int quotesCount = context.Fragment.ToString().Length - text.Length;
            text = text.TrimEnd('\'');
            LiteralControl lc = new LiteralControl(text);
            Control c;
            if (quotesCount == 2)
            {
                c = Wrap("em", lc);
            }
            else if (quotesCount == 3)
            {
                c = Wrap("strong", lc);
            }
            else
            {
                c = Wrap("strong", Wrap("em", lc));
            }
            container.Controls.Add(c);
            return c;
        }

        private static Control Wrap(string tag, Control inner)
        {
            HtmlGenericControl c = new HtmlGenericControl(tag);
            c.Controls.Add(inner);
            return c;
        }

        #endregion
    }
}
