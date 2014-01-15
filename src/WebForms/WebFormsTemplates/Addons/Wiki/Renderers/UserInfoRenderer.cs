using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Addons.Wiki.Renderers
{
    public class UserInfoRenderer : IRenderer
    {
        #region IControlRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            Literal l = new Literal();
            var text = context.Fragment.ToString();
            if (text.Length <= 3)
                l.Text = context.Article.SavedBy;
            else if (text.Length >= 5)
                l.Text = context.Article.Updated.ToString();
            else
                l.Text = context.Article.SavedBy + ", " + context.Article.Updated.ToString();
            container.Controls.Add(l);
            return l;
        }

        #endregion
    }
}
