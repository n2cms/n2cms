using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace N2.Addons.Wiki.Renderers
{
    public class LineRenderer : IRenderer
    {
        Regex paragraph = new Regex(@"</?\s*p(\s[^>]>|>)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
        #region IRenderer Members

        public Control AddTo(Control container, ViewContext context)
        {
            string html = context.Fragment.ToString();
            if (paragraph.IsMatch(html))
                html = html.Trim();
            LiteralControl br = new LiteralControl(html.Replace(Environment.NewLine, "<br/>"));
            container.Controls.Add(br);
            return br;
        }

        #endregion
    }
}
