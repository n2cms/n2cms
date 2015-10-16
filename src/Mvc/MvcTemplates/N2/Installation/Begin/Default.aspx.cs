using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Configuration;
using N2.Web;
using System.Text;
using System.IO;
using N2.Configuration;
using System.Text.RegularExpressions;

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
            installationAllowed = AllowInstallationOption.Parse(config.AllowInstallation) != AllowInstallationOption.No;

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

            System.Configuration.Configuration cfg;
            try
            {
                if (FormsAuthentication.Authenticate("admin", "changeme"))
                    needsPasswordChange = true;
                else
                {
                    if (TryOpenWebConfiguration(out cfg))
                    {
                        var authentication = (AuthenticationSection)cfg.GetSection("system.web/authentication");
                        needsPasswordChange = authentication.Forms.Credentials.Users["admin"] == null 
                            && AllowInstallationOption.Parse(config.AllowInstallation) == AllowInstallationOption.AnonymousUser;
                    }
                }
            }
            catch (Exception)
            {
            }

            autoLogin = Request["autologin"] == "true";
            if (autoLogin)
                return;

            if (!TryOpenWebConfiguration(out cfg))
            {
                FormsAuthentication.SetAuthCookie("admin", false);
                Response.Redirect(Url.Parse(Request.RawUrl).AppendQuery("autologin", "true"));
            }
        }

	    protected override void OnLoad(EventArgs e)
	    {
		    base.OnLoad(e);
			Page.Header.DataBind();
	    }

	    protected void OkCommand(object sender, CommandEventArgs args)
        {
            try
            {
                System.Configuration.Configuration cfg;
                if (TryOpenWebConfiguration(out cfg))
                {
                    var authentication = (AuthenticationSection)cfg.GetSection("system.web/authentication");
                    if (chkLoginUrl.Checked)
                    {
                        authentication.Forms.LoginUrl = "~/N2/Login.aspx";
                        authentication.Mode = AuthenticationMode.Forms;
                    }

                    authentication.Forms.Credentials.PasswordFormat = FormsAuthPasswordFormat.SHA1;
                    if (authentication.Forms.Credentials.Users["admin"] != null)
                        authentication.Forms.Credentials.Users["admin"].Password = ComputeSHA1Hash(txtPassword.Text);
                    else
                        authentication.Forms.Credentials.Users.Add(new FormsAuthenticationUser("admin", ComputeSHA1Hash(txtPassword.Text)));

                    var membership = (MembershipSection)cfg.GetSection("system.web/membership");
                    if (membership.Providers["ContentMembershipProvider"] != null)
                        membership.DefaultProvider = "ContentMembershipProvider";

                    var roleManager = (RoleManagerSection)cfg.GetSection("system.web/roleManager");
                    if (roleManager.Providers["ContentRoleProvider"] != null)
                    {
                        roleManager.Enabled = true;
                        roleManager.DefaultProvider = "ContentRoleProvider";
                    }

                    try
					{
						if (Roles.Enabled)
						{
							if (!Roles.RoleExists("Administrators"))
								Roles.CreateRole("Administrators");
							Roles.AddUserToRole("admin", "Administrators");
						}
                    }
                    catch (Exception)
                    {
                        //TODO: Needs testing; once tested, please remove this try/catch statement.
                    }

                    var profile = (ProfileSection)cfg.GetSection("system.web/profile");
                    if (profile.Providers["ContentProfileProvider"] != null)
                        profile.DefaultProvider = "ContentProfileProvider";

                    cfg.Save();

					try
					{
						if (chkLoginUrl.Checked)
						{
							var webConfigPath = Server.MapPath("~/web.config");
							var webConfigContent = File.ReadAllText(webConfigPath);
							webConfigContent = Regex.Replace(webConfigContent, "<remove\\s+name=\"FormsAuthentication\"\\s(/>)|(>\\s?</remove>)", "<!--<remove name=\"FormsAuthentication\" />-->");
							File.WriteAllText(webConfigPath, webConfigContent);
						}
					}
					catch (Exception)
					{
					}

                    FormsAuthentication.SetAuthCookie("admin", false);
                    Response.Redirect(continueUrl);
                }
                else
                {
                    ReportManualConfigChange();
                }
            }
            catch (Exception ex)
            {
                ReportManualConfigChange();
                cvSave.ToolTip = ex.ToString();
            }
            needsPasswordChange = false;
        }

        private void ReportManualConfigChange()
        {
            cvSave.IsValid = false;
            cvSave.Text = "Unable to save forms user password and membership providers, please modify web.config manually";

            preManualConfig.Visible = true;
            preManualConfig.InnerHtml = Server.HtmlEncode(string.Format(@"<system.web>
    <authentication mode=""Forms"">
        <forms loginUrl=""N2/Login.aspx"">
        <credentials passwordFormat=""SHA1"">
            <user name=""admin"" password=""{0}"" />
        </credentials>
        </forms>
    </authentication>

    <membership defaultProvider=""ContentMembershipProvider"">
        <providers>
            <clear />
            <add passwordFormat=""Hashed"" 
                name=""ContentMembershipProvider"" 
                type=""N2.Security.ContentMembershipProvider, N2.Management"" />
        </providers>
    </membership>
    <roleManager enabled=""true"" defaultProvider=""ContentRoleProvider"">
        <providers>
            <clear />
            <add name=""ContentRoleProvider"" 
                type=""N2.Security.ContentRoleProvider, N2.Management"" />
        </providers>
    </roleManager>
    <profile defaultProvider=""ContentProfileProvider"">
        <providers>
            <clear />
            <add name=""ContentProfileProvider"" 
                type=""N2.Security.ContentProfileProvider, N2.Management"" />
        </providers>
    </profile>", ComputeSHA1Hash(txtPassword.Text)));
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

        protected void chkLoginUrl_CheckedChanged(object sender, EventArgs e)
        {
            pnlPassword.Visible = chkLoginUrl.Checked;
            pnlNoPassword.Visible = !chkLoginUrl.Checked;
        }
    }
}
