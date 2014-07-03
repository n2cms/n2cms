using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.UI.WebControls;
using N2.Edit.Installation;
using N2.Management.Installation;
using N2.Engine;

namespace N2.Edit.Install
{
    public partial class Upgrade : System.Web.UI.Page
    {
        protected IEngine Engine
        {
            get { return N2.Context.Current; }
        }

        protected InstallationManager Installer
        {
            get { return Engine.Resolve<InstallationManager>(); }
        }
        protected MigrationEngine Migrator
        {
            get { return Engine.Resolve<MigrationEngine>(); }
        }
        protected InstallationChecker Checker
        {
            get { return Engine.Resolve<InstallationChecker>(); }
        }

        protected override void OnInit(EventArgs e)
        {
            InstallationUtility.CheckInstallationAllowed(Context);

            base.OnInit(e);

			// TODO: cache the applicability checks

            foreach(var m in GetAllMigrations())
            {
				cblMigrations.Items.Add(new ListItem(m.Title, m.GetType().Name) { Selected = true /* m.TryApplicable(Checker.Status) ?? true */ });
            }
        }

	    private IEnumerable<AbstractMigration> GetAllMigrations()
	    {
		    Object o = Cache["AllMigrations"];
		    if (o != null && o is IEnumerable<AbstractMigration>)
			    return (IEnumerable<AbstractMigration>) o;
			var m = Migrator.GetAllMigrations();
		    Cache.Add("AllMigrations", m, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 5, 0), CacheItemPriority.High,
			    null);
			return m;
		}

	    protected void btnInstallAndMigrate_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() => ShowResults(Migrator.UpgradeAndMigrate()));
            Checker.Status = null;
            Installer.UpdateStatus(Checker.Status.Level);
        }

        protected void btnMigrate_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(() =>
            {
	            var results = new List<MigrationResult>();
				foreach (var m in GetAllMigrations())
				{
					if (cblMigrations.Items.FindByValue(m.GetType().Name).Selected)
					{
						Session["InstallProgress"] = "Running migration: " + m.Title;
						results.Add(m.Migrate(Checker.Status));
					}
				}
                ShowResults(results);
                });
            Checker.Status = null;
        }

        protected void btnInstall_Click(object sender, EventArgs e)
        {
            ExecuteWithErrorHandling(Installer.Upgrade);
	        Checker.Status = null;
        }

	    protected void ShowProgress()
	    {
		    TabPanel1.Visible = false;
		    tpProgress.Visible = true;
		    lblProgress.Text = (string)Session["InstallProgress"] ?? "null";
	    }

	    protected void HideProgress()
	    {
		    TabPanel1.Visible = true;
		    tpProgress.Visible = false;
			errorLabel.Text = FormatException((Exception)Session["InstallException"]);
			errorLabel.Visible = true;

			Checker.Status = null;
			Installer.UpdateStatus(Checker.Status.Level);
	    }

	    protected void RefreshProgress(object sender, EventArgs e)
		{
			Checker.Status = null;
			Installer.UpdateStatus(Checker.Status.Level);
		}

		protected void ExecuteWithErrorHandling(Action action)
		{
			ShowProgress();
			if (Cache["busy"] != null)
			{
				HideProgress();
				errorLabel.Text = "A maintenance operation is already in progress.";
				errorLabel.Visible = true;
				return;
			}
			Session["InstallProgress"] = "Waiting for migration to start";
			Cache.Add("busy", "busy", null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
			ThreadPool.QueueUserWorkItem((obj) =>
			{
				try
				{
					Cache["InstallProgress"] = "Preparing to migrate";
					action();
				}
				catch (Exception ex)
				{
					Session["InstallException"] = ex;
				}
				finally
				{
					Cache.Remove("busy");
				}
				HideProgress();
				Session.Remove("InstallProgress");
			});
		}

        private void ShowResults(IEnumerable<MigrationResult> results)
        {
            StringBuilder errorText = new StringBuilder();
            lblResult.Text += "<ul>";
            foreach (var result in results)
            {
                string message = result.Migration.Title + " executed updating " + result.UpdatedItems + " items.";
                foreach(string error in result.Errors)
                {
                    errorText.AppendFormat("{0} error: {1}<br/>", result.Migration.Title, error);
                }

                lblResult.Text += string.Format("<li class='{0}'>{1}{2}</li>", 
                    /*0*/ result.Errors.Count > 0 ? "warning" : "ok",
                    /*1*/ message,
                    /*2*/ string.IsNullOrEmpty(result.RedirectTo) ? "" : string.Format(" <a href='{0}' target='{1}'>Click to complete this migration on separate page</a>", N2.Web.Url.ResolveTokens(result.RedirectTo), result.Migration.GetType().Name));

            }
            lblResult.Text += "</ul>";

            errorLabel.Text = errorText.ToString();
            errorLabel.Visible = !string.IsNullOrEmpty(errorLabel.Text);
        }


        protected void btnExportSchema_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=n2_upgrade.sql");

            InstallationManager im = Installer;
            Response.Write(im.ExportUpgradeSchema());
            Response.End();
        }

        private static string FormatException(Exception ex)
        {
            if (ex == null)
                return "Unknown error";
            return "<b>" + ex.Message + "</b>" + ex.StackTrace;
        }

    }
}
