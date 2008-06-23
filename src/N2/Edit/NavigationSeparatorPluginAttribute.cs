using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace N2.Edit
{
    /// <summary>
    /// Adds a separator to the items in the right-click menu in the navigation
    /// pane.
    /// </summary>
    public class NavigationSeparatorPluginAttribute : NavigationPluginAttribute
    {
        public NavigationSeparatorPluginAttribute(string name, int sortOrder)
        {
            Name = name;
            SortOrder = sortOrder;
        }

        public override Control AddTo(Control container)
        {
            HtmlGenericControl hgc = new HtmlGenericControl("div");
            hgc.Attributes["class"] = "separator";
            container.Controls.Add(hgc);
            return hgc;
        }
    }
}
