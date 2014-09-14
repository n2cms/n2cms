using System;
using System.Collections.Generic;
using N2.Edit.Installation;

namespace N2.Management.Installation
{
    public partial class Rebase : System.Web.UI.Page
    {
        protected IEnumerable<RebaseInfo> RebasedLinks;
        protected InstallationManager Installer
        {
            get { return N2.Context.Current.Resolve<InstallationManager>(); }
        }

        private DatabaseStatus status;
        protected DatabaseStatus Status
        {
            get
            {
                if (status == null)
                    status = Installer.GetStatus();
                return status;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            InstallationUtility.CheckInstallationAllowed(Context);

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			Header.DataBind();
        }

        protected void btnRebase_Click(object sender, EventArgs e)
        {
            var rebaser = N2.Context.Current.Resolve<AppPathRebaser>();
            RebasedLinks = rebaser.Rebase(Status.AppPath, ResolveUrl("~/"));
        }
    }
}
