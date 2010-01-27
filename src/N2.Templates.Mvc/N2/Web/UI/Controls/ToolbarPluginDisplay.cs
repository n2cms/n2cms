using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Web.UI.WebControls;
using N2.Engine;
using N2.Web;

namespace N2.Edit.Web.UI.Controls
{
    public class ToolbarPluginDisplay : Control
    {
        public ToolbarArea Area { get; set; }
        public IEngine Engine { get { return N2.Context.Current; } }
        public SelectionUtility Selection { get { return new SelectionUtility(this, Engine); } }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            foreach (ToolbarPluginAttribute plugin in Engine.EditManager.GetPlugins<ToolbarPluginAttribute>(Engine.Resolve<IWebContext>().User))
            {
                if ((plugin.Area & Area) != Area)
                    continue;

                HtmlGenericControl command = new HtmlGenericControl("div");
                command.Attributes["id"] = plugin.Name;
                command.Attributes["class"] = "item";
                Controls.Add(command);

				plugin.AddTo(command, new PluginContext(Selection.SelectedItem, null, ControlPanelState.Visible, Engine.EditManager.GetManagementInterfaceUrl()));
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='" + Area.ToString().ToLower() + " toolbox'>");
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}
