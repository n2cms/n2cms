using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using N2.Edit.Installation;
using N2.Management.Installation;
using N2.Persistence;

namespace N2.Edit.Install
{
    public partial class UpgradeVersions : System.Web.UI.Page
    {
        protected InstallationManager Installer
        {
            get { return N2.Context.Current.Resolve<InstallationManager>(); }
        }
        protected MigrationEngine Migrator
        {
            get { return N2.Context.Current.Resolve<MigrationEngine>(); }
        }
        protected IContentItemRepository Repository
        {
            get { return N2.Context.Persister.Repository; }
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
        }

	    protected override void OnLoad(EventArgs e)
	    {
		    base.OnLoad(e);
			Header.DataBind();
	    }

	    protected override void OnPreInit(EventArgs e)
        {
	        var c = Repository.Find(N2.Persistence.Parameter.GreaterThan("VersionOf.ID", 0));


	        rptVersions.DataSource = c.OrderBy(o => o.Published).Take(1000);
            rptVersions.DataBind();

            base.OnPreInit(e);
        }

        protected void btnInstallAndMigrate_Click(object sender, EventArgs e)
        {
        }

        protected void btnMigrate_Click(object sender, EventArgs e)
        {
        }

        private void ShowResults(IEnumerable<MigrationResult> results)
        {
        }

        protected void btnInstall_Click(object sender, EventArgs e)
        {
        }

        protected void btnExportSchema_Click(object sender, EventArgs e)
        {
        }



        protected Exception ExecuteWithErrorHandling(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception ex)
            {
                errorLabel.Text = FormatException(ex);
                return ex;
            }
        }

        private static string FormatException(Exception ex)
        {
            if (ex == null)
                return "Unknown error";
            return "<b>" + ex.Message + "</b>" + ex.StackTrace;
        }
    }
}
