using System.Web.UI;
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
            IsDivider = true;
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            HtmlGenericControl hgc = new HtmlGenericControl("div");
            hgc.Attributes["class"] = "separator";
            container.Controls.Add(hgc);
            return hgc;
        }
    }
}
