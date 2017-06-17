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
using N2.Web;
using N2.Edit.Workflow;
using N2.Edit.Versioning;

namespace N2.Management.Content
{
    public partial class EditSingle : EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            bool isPublicableByUser = Engine.SecurityManager.IsAuthorized(User, ie.CurrentItem, Permission.Publish);
            btnSave.Enabled = isPublicableByUser;

            if (Request["cancel"] == "reloadTop")
                hlCancel.NavigateUrl = "javascript:window.top.location.reload();";
            else
                hlCancel.NavigateUrl = CancelUrl();

            ie.EditableNameFilter = new [] { Request["property"] };
            ie.CurrentItem = Selection.SelectedItem;
            EnsureChildControls();
        }

        protected void OnPublishCommand(object sender, CommandEventArgs args)
        {
            var ctx = ie.CreateCommandContext();
            Engine.Resolve<CommandDispatcher>().Save(ctx);
            var returnUrl = ctx.Content.Url.ToUrl()
                .SetQueryParameter(PathData.VersionIndexQueryKey, ctx.Content.VersionIndex)
                .SetQueryParameter("edit", "drag")
				.SetQueryParameter("n2reveal", Request["n2reveal"]);

            Refresh(Selection.SelectedItem, returnUrl);
        }
    }
}
