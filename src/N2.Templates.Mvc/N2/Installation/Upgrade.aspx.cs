using System;
using N2.Installation;
using N2.Edit.Installation;

namespace N2.Edit.Install
{
	public partial class Upgrade : System.Web.UI.Page
	{
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

		protected void btnInstall_Click(object sender, EventArgs e)
		{
			ExecuteWithErrorHandling(Installer.Upgrade);
			status = null;
		}

		protected void btnExportSchema_Click(object sender, EventArgs e)
		{
			Response.ContentType = "application/octet-stream";
			Response.AddHeader("Content-Disposition", "attachment;filename=n2_upgrade.sql");

			InstallationManager im = Installer;
			Response.Write(im.ExportUpgradeSchema());
			Response.End();
		}



		protected Exception ExecuteWithErrorHandling(_Default.Execute action)
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
