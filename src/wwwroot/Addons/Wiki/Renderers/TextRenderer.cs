using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Addons.Wiki.Renderers
{
    public class TextRenderer : IRenderer
    {
        #region IControlRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            Literal l = new Literal();
            l.Text = context.Fragment.Value;
            container.Controls.Add(l);
            return l;
        }

        #endregion
    }
}
