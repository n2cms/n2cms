using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Configuration;
using N2.Web;
using System.Text;
using System.IO;

namespace N2.Edit.Install.Begin
{
	public partial class Default : System.Web.UI.Page
	{
		protected bool installationAllowed = true;
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
			installationAllowed = config.AllowInstallation;

			continueUrl = action == "install"
				? config.InstallUrl
				: action == "upgrade"
					? config.UpgradeUrl
					: action == "rebase"
						? config.RebaseUrl
						: action == "fixClass"
							? config.FixClassUrl.ToUrl().AppendQuery("id", Request["id"]).ToString()
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
					var authentication = (AuthenticationSection)cfg.GetSection("system.web/authentication");
					if(chkLoginUrl.Checked)
						authentication.Forms.LoginUrl = "N2/Login.aspx";

					authentication.Forms.Credentials.PasswordFormat = FormsAuthPasswordFormat.SHA1;
					authentication.Forms.Credentials.Users["admin"].Password = ComputeSHA1Hash(txtPassword.Text);

					var membership = (MembershipSection)cfg.GetSection("system.web/membership");
					if (membership.Providers["ContentMembershipProvider"] != null)
						membership.DefaultProvider = "ContentMembershipProvider";

					var roleManager = (RoleManagerSection)cfg.GetSection("system.web/roleManager");
					if (roleManager.Providers["ContentRoleProvider"] != null)
					{
						roleManager.Enabled = true;
						roleManager.DefaultProvider = "ContentRoleProvider";
					}

					var profile = (ProfileSection)cfg.GetSection("system.web/profile");
					if (profile.Providers["ContentProfileProvider"] != null)
						profile.DefaultProvider = "ContentProfileProvider";

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

		public string ComputeSHA1Hash(string input)
		{
			var encrypter = new System.Security.Cryptography.SHA1CryptoServiceProvider();
			using (var sw = new StringWriter())
			{
				foreach (byte b in encrypter.ComputeHash(Encoding.UTF8.GetBytes(input)))
					sw.Write(b.ToString("x2"));
				return sw.ToString();
			}
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
				Engine.Logger.Warn(ex);
				return false;
			}
		}
	}
}
