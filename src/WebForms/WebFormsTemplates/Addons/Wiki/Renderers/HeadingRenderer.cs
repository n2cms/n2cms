using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Addons.Wiki.Renderers
{
    public class HeadingRenderer : IRenderer
    {
        public Control AddTo(Control container, ViewContext context)
        {
            int level = context.Fragment.Tokens.First().Fragment.Length;
            string text = context.Fragment.ToString().Trim('=');
            
            Hn hn = new Hn();
            hn.Level = level;
            hn.Text = text;
            container.Controls.Add(hn);
            return hn;
        }
    }
}
