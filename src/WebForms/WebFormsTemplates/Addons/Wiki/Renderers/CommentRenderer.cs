using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Addons.Wiki.Renderers
{
    public class CommentRenderer : IRenderer
    {
        #region IControlRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            Literal l = new Literal();
            var text = context.Fragment.ToString();
            l.Text = text.Substring(1, text.Length - 2);
            container.Controls.Add(l);
            return l;
        }

        #endregion
    }
}
