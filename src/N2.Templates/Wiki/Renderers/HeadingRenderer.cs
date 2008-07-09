using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Templates.Wiki.Renderers
{
    public class HeadingRenderer : IRenderer
    {
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            Hn hn = new Hn();
            string text = context.Fragment.Value.TrimStart('=');
            int level = context.Fragment.Value.Length - text.Length;
            text = text.TrimEnd('=');

            hn.Level = level;
            hn.Text = text;
            container.Controls.Add(hn);
            return hn;
        }

        #endregion
    }
}
