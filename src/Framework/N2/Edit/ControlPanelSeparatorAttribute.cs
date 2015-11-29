using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// A vertical separator line for the control panel plugin area.
    /// </summary>
    public class ControlPanelSeparatorAttribute : AdministrativePluginAttribute, IControlPanelPlugin
    {
        public ControlPanelSeparatorAttribute(int sortOrder, ControlPanelState showDuring)
        {
            this.ShowDuring = showDuring;
            SortOrder = sortOrder;
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            if(!ActiveFor(container, context.State))
                return null;

            HtmlImage img = new HtmlImage();
            img.Src = Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/Img/separator.png");
            img.Attributes["class"] = "separator";
            img.Height = 16;
            img.Width = 1;
            img.Alt = "|";
            container.Controls.Add(img);
            return img;
        }

        protected virtual bool ActiveFor(Control container, ControlPanelState state)
        {
            return (ShowDuring & state) == state;
        }
    }
}
