using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Configuration;
using N2.Web;

namespace N2.Edit.Install.Begin
{
	public partial class Default : System.Web.UI.Page
	{
		protected bool needsPasswordChange = false;
		protected bool autoLogin = false;
		protected string continueUrl;
		protected string action;
		protected Version version;
		protected N2.Configuration.InstallerElement config;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			action = Request["action"];
			version = typeof(N2.ContentItem).Assembly.GetName().Version;
			config = N2.Context.Current.Resolve<N2.Configuration.EditSection>().Installer;

			continueUrl = action == "install"
									? config.InstallUrl
									: action == "upgrade"
										? config.UpgradeUrl
										: action == "rebase"
											? config.RebaseUrl
											: config.InstallUrl;

			continueUrl = N2.Web.Url.ResolveTokens(continueUrl);

			needsPasswordChange = FormsAuthentication.Authenticate("admin", "changeme");

			autoLogin = Request["autologin"] == "true";
			if (autoLogin)
				return;

			System.Configuration.Configuration cfg;
			if (!TryOpenWebConfiguration(out cfg))
			{
				FormsAuthentication.SetAuthCookie("admin", false);
				Response.Redirect(Url.Parse(Request.RawUrl).AppendQuery("autologin", "true"));
			}
		}

		protected void OkCommand(object sender, CommandEventArgs args)
		{
			try
			{
				System.Configuration.Configuration cfg;
				if (TryOpenWebConfiguration(out cfg))
				{
					AuthenticationSection authentication = cfg.GetSection("system.web/authentication") as AuthenticationSection;
					if(chkLoginUrl.Checked)
						authentication.Forms.LoginUrl = "N2/Login.aspx";
					authentication.Forms.Credentials.Users["admin"].Password = txtPassword.Text;

					cfg.Save();

					if (FormsAuthentication.Authenticate("admin", txtPassword.Text))
					{
						FormsAuthentication.SetAuthCookie("admin", false);
						Response.Redirect(continueUrl);
					}
				}
				else
				{
					cvSave.Text = "Unable to save password, please modify web.config manually";
				}
			}
			catch (Exception ex)
			{
				cvSave.Text = "Unable to save password, please modify web.config manually";
				cvSave.ToolTip = ex.ToString();
			}
			needsPasswordChange = false;
		}

		private static bool TryOpenWebConfiguration(out System.Configuration.Configuration configuration)
		{
			try
			{
				configuration = WebConfigurationManager.OpenWebConfiguration("~");
				return true;
			}
			catch (Exception ex)
			{
				configuration = null;
				System.Diagnostics.Trace.TraceWarning(ex.ToString());
				return false;
			}
		}
	}
}
