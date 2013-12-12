using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Security;
using N2.Edit.Web;
using N2.Web.UI.WebControls;
using N2.Web.UI;
using N2.Edit.Workflow;

namespace N2.Management.Content
{
    [ToolbarPlugin("SITE", "site", "{ManagementUrl}/Content/EditRecursive.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, Targets.Preview, "{ManagementUrl}/Resources/icons/database_gear.png", 50, ToolTip = "edit",
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Write,
        Legacy = true)]
    public partial class EditRecursive : EditPage, IRecursiveContainerInterface
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (var a in Find.EnumerateParents(Selection.SelectedItem, null, true))
            {
                var ie = new ItemEditor();
                phAncestors.Controls.Add(ie);
                ie.ContainerTypeFilter = typeof(RecursiveContainerAttribute);
                ie.CurrentItem = a;
            }
        }

        protected void OnPublishCommand(object sender, CommandEventArgs args)
        {
            foreach (var ie in phAncestors.Controls.OfType<ItemEditor>())
            {
                Engine.Resolve<CommandDispatcher>().Publish(ie.CreateCommandContext());
            }
            Refresh(Selection.SelectedItem);
        }
    }
}
