using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Definitions;
using N2.Edit.Installation;
using N2.Engine;
using N2.Installation;
using N2.Management.Installation;

namespace N2.Edit.Install
{
    public partial class _Default : Page
    {
        protected CustomValidator cvExisting;
        protected Label errorLabel;
        protected FileUpload fileUpload;

        protected int RootId
        {
            get { return (int)(ViewState["rootId"] ?? 0); }
            set { ViewState["rootId"] = value; }
        }
        protected int StartId
        {
            get { return (int)(ViewState["startId"] ?? 0); }
            set { ViewState["startId"] = value; }
        }

        protected RadioButtonList rblExports;

        protected InstallationManager Installer
        {
            get { return Engine.Resolve<InstallationManager>(); }
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

        protected IEngine Engine { get { return N2.Context.Current; } }

        protected override void OnInit(EventArgs e)
        {
            InstallationUtility.CheckInstallationAllowed(Context);
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
			Header.DataBind();
            if (!IsPostBack)
            {
                try
                {
                    IEnumerable<ItemDefinition> rootDefinitions = Installer.GetRootDefinitions(Engine.Definitions.GetDefinitions());
                    LoadRootTypes(ddlRoot, rootDefinitions, "[root node]");

                    IEnumerable<ItemDefinition> startDefinitions = Installer.GetStartDefinitions(Engine.Definitions.GetDefinitions());
                    LoadStartTypes(ddlStartPage, startDefinitions, "[start node]");

                    IEnumerable<ItemDefinition> startAndStartDefinitions = Installer.GetRootAndStartDefinitions(Engine.Definitions.GetDefinitions());
                    LoadRootTypes(ddlRootAndStart, startAndStartDefinitions, "[root and start node]");

                    LoadExistingExports();
                }
                catch (Exception ex)
                {
                    ltStartupError.Text = "<li style='color:red'>Ooops, something is wrong: " + ex.Message + "</li>";
                    return;
                }
            }
        }

        protected override void OnError(EventArgs e)
        {
            errorLabel.Text = FormatException(Server.GetLastError());
        }

        private void LoadExistingExports()
        {
            string dir = HostingEnvironment.MapPath("~/App_Data");
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*.gz");
                foreach (string file in files)
                {
                    string value = Path.GetFileName(file);
                    string text = value.Substring(0, value.Length - 3).Replace('_', ' ').Replace('.', ' ');
                    string descriptionPath = Path.Combine(dir, value) + ".txt";
                    if (File.Exists(descriptionPath))
                    {
                        text = File.ReadAllText(descriptionPath);
                    }
                    rblExports.Items.Add(new ListItem(text, value));
                }
                if (files.Length > 0)
                    rblExports.SelectedIndex = 0;
            }

            btnInsertExport.Enabled = rblExports.Items.Count > 0;
        }

        private void LoadStartTypes(ListControl lc, IEnumerable<ItemDefinition> startPageDefinitions, string initialText)
        {
            lc.Items.Clear();
            lc.Items.Add(initialText);
            foreach (ItemDefinition d in startPageDefinitions)
            {
                lc.Items.Add(new ListItem(d.Title, d.ItemType.AssemblyQualifiedName));
            }
        }

        private static void LoadRootTypes(ListControl lc, IEnumerable<ItemDefinition> rootDefinitions, string initialText)
        {
            lc.Items.Clear();
            lc.Items.Add(initialText);
            foreach (ItemDefinition d in rootDefinitions)
            {
                lc.Items.Add(new ListItem(d.Title, d.ItemType.AssemblyQualifiedName));
            }
        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            string stackTrace;
            lblStatus.Text = Installer.CheckConnection(out stackTrace);

            if (string.IsNullOrEmpty(lblStatus.Text))
            {
                lblStatus.CssClass = "ok";
                lblStatus.Text = "Connection OK";
            }
            else
            {
                lblStatus.CssClass = "warning";
                lblStatus.ToolTip = stackTrace;
            }
        }


        protected void btnCreateSqlCeFile_Click(object sender, EventArgs e)
        {
            SqlCEDbHelper.CreateDatabaseFile(Engine.Resolve<N2.Configuration.ConfigurationManagerWrapper>().GetConnectionString());
        }

        protected void btnInstall_Click(object sender, EventArgs e)
        {
            InstallationManager im = Installer;
            if (Request.QueryString["export"] == "true")
            {
                im.ExportSchema(Response.Output);
                Response.End();
            }
            else
            {
                if (ExecuteWithErrorHandling(im.Install) != null)
                {
                    if (ExecuteWithErrorHandling(im.Install) == null)
                    {
                        // retry once upon error
                        lblInstall.Text = "Database created, now insert root items.";
                        lblInstall.CssClass = "alert alert-success";
                        ShowTab("Content");
                    }
                }
                else
                    ShowTab("Content");
            }
        }

        protected void btnExportSchema_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=n2_create.sql");

            InstallationManager im = Installer;
            im.ExportSchema(Response.Output);

            Response.End();
        }
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            InstallationManager im = Installer;

            try
            {
                cvRootAndStart.IsValid = ddlRoot.SelectedIndex > 0 && ddlStartPage.SelectedIndex > 0;
                cvRoot.IsValid = true;
                if (!cvRootAndStart.IsValid)
                    return;

                ContentItem root = im.InsertRootNode(Type.GetType(ddlRoot.SelectedValue), "root", "Root Node");
                ContentItem startPage = im.InsertStartPage(Type.GetType(ddlStartPage.SelectedValue), root, "start", "Start Page");

                if (startPage.ID == Status.StartPageID && root.ID == Status.RootItemID)
                {
                    ltRootNode.Text = "<span class='ok'>Root and start pages inserted.</span>";
                    Installer.UpdateStatus(SystemStatusLevel.UpAndRunning);
                }
                else
                {
                    ltRootNode.Text = string.Format(
                        "<span class='warning'>Start page inserted but you must update web.config with root item id: <b>{0}</b> and start page id: <b>{1}</b></span>", root.ID, startPage.ID);
                    phSame.Visible = false;
                    phDiffer.Visible = true;
                    RootId = root.ID;
                    StartId = startPage.ID;
                }
                phDiffer.DataBind();
            }
            catch (Exception ex)
            {
                ltRootNode.Text = string.Format("<span class='warning'>{0}</span><!--\n{1}\n-->", ex.Message, ex);
            }
        }
        protected void btnInsertRootOnly_Click(object sender, EventArgs e)
        {
            InstallationManager im = Installer;

            try
            {
                cvRootAndStart.IsValid = true;
                cvRoot.IsValid = ddlRootAndStart.SelectedIndex > 0;
                if (!cvRoot.IsValid)
                    return;

                ContentItem root = im.InsertRootNode(Type.GetType(ddlRootAndStart.SelectedValue), "start", "Start Page");
                
                if (root.ID == Status.RootItemID && root.ID == Status.StartPageID)
                {
                    ltRootNode.Text = "<span class='ok'>Root node inserted.</span>";
                    phSame.Visible = false;
                    phDiffer.Visible = false;
                    RootId = root.ID;
                    Installer.UpdateStatus(SystemStatusLevel.UpAndRunning);
                    ShowTab("Finish");
                }
                else
                {
                    ltRootNode.Text = string.Format(
                        "<span class='warning'>Root node inserted but you must update web.config with root item id: <b>{0}</b></span> ",
                        root.ID);
                    phSame.Visible = true;
                    phDiffer.Visible = false;
                    RootId = root.ID;
                    StartId = root.ID;
                }
                phSame.DataBind();
            }
            catch (Exception ex)
            {
                ltRootNode.Text = string.Format("<span class='warning'>{0}</span><!--\n{1}\n-->", ex.Message, ex);
            }
        }

        protected void btnInsertExport_Click(object sender, EventArgs e)
        {
            cvExisting.IsValid = rblExports.SelectedIndex >= 0;
            if (!cvExisting.IsValid)
                return;

            string path = Path.Combine(HostingEnvironment.MapPath("~/App_Data"), rblExports.SelectedValue);
            if (ExecuteWithErrorHandling(delegate { InsertFromFile(path); }) == null)
            {
                plhAddContent.Visible = false;
            }
        }

        private void InsertFromFile(string path)
        {
            InstallationManager im = Installer;
            using (Stream read = File.OpenRead(path))
            {
                ContentItem root = im.InsertExportFile(read, path);
                InsertRoot(root);
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            rfvUpload.IsValid = fileUpload.PostedFile != null && fileUpload.PostedFile.FileName.Length > 0;
            if (!rfvUpload.IsValid)
                return;

            ExecuteWithErrorHandling(InstallFromUpload);
        }

        protected void btnUpdateWebConfig_Click(object sender, EventArgs e)
        {
            if (ExecuteWithErrorHandling(SaveConfiguration) == null)
            {
                lblWebConfigUpdated.Text = "Configuration updated.";
                status = null;
                Engine.Host.DefaultSite.RootItemID = RootId;
                Engine.Host.DefaultSite.StartPageID = StartId;

                ShowTab("Finish");
                Installer.UpdateStatus(SystemStatusLevel.UpAndRunning);
            }
            else
            {
                lblWebConfigUpdated.Text = "Failed to update configuration. Please update it manually.";
            }
        }

        private void SaveConfiguration()
        {
            System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration("~");

            HostSection host = (HostSection)cfg.GetSection("n2/host");
            host.RootID = RootId;
            host.StartPageID = StartId;

            cfg.Save();
        }

        protected void btnRestart_Click(object sender, EventArgs e)
        {
            try
            {
                System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration("~");
                EditSection edit = (EditSection)cfg.GetSection("n2/edit");
                edit.Installer.AllowInstallation = AllowInstallationOption.No;
                cfg.Save();

                Response.Redirect(Engine.ManagementPaths.GetManagementInterfaceUrl());
            }
            catch
            {
                ltDisableFailed.Text = "Failed writing to web.config, please change this manually in web.config:";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
            DataBind();
        }

        protected string GetStatusText()
        {
            if (Status.IsInstalled)
                return "Installation looks fine (just review <a href='#Finish'>step 3</a>).";
            if (status.NeedsUpgrade)
                return "Switch to the <a href='Upgrade.aspx'>upgrade wizard</a> to upgrade from a previous version of the database";
            if (Status.HasSchema)
                return "Create content on <a href='#Content'>step 2</a>.";
            if (Status.IsConnected)
                return "Create database tables on <a href='#Database'>step 1</a>.";

            return "Check connection on <a href='#Database'>step 1</a>.";
        }

        private void InstallFromUpload()
        {
            InstallationManager im = Installer;
            ContentItem root = im.InsertExportFile(fileUpload.FileContent, fileUpload.FileName);

            InsertRoot(root);
        }

        private void InsertRoot(ContentItem root)
        {
            if (root.ID == Status.RootItemID)
            {
                ltRootNode.Text = "<span class='ok'>Root node inserted.</span>";
                phSame.Visible = false;
                phDiffer.Visible = false;
            }
            else
            {
                ltRootNode.Text = string.Format(
                    "<span class='warning'>Root node inserted but you must update web.config with root item id: <b>{0}</b></span> ",
                    root.ID);
                phSame.Visible = true;
                phDiffer.Visible = false;
                RootId = root.ID;
                StartId = root.ID;
                phSame.DataBind();
            }

            // try to find a suitable start page
            foreach (ContentItem item in root.Children)
            {
                ItemDefinition id = Engine.Definitions.GetDefinition(item);
                if (InstallationManager.Is(id.Installer, InstallerHint.PreferredStartPage))
                {
                    if (item.ID == Status.StartPageID && root.ID == Status.RootItemID)
                    {
                        ltRootNode.Text = "<span class='ok'>Root and start page inserted.</span>";
                        Installer.UpdateStatus(SystemStatusLevel.UpAndRunning);
                    }
                    else
                    {
                        ltRootNode.Text = string.Format(
                            "<span class='warning'>Start page inserted but you must update web.config with root item id: <b>{0}</b> and start page id: <b>{1}</b></span>", root.ID, item.ID);
                        phSame.Visible = false;
                        phDiffer.Visible = true;
                        StartId = item.ID;
                        RootId = root.ID;
                    }
                    break;
                }
                phDiffer.DataBind();
            }

            if (!phSame.Visible && !phDiffer.Visible)
                ShowTab("Finish");

            Engine.Resolve<N2.Persistence.Search.IContentIndexer>().Update(root);
        }

        protected Exception ExecuteWithErrorHandling(Action action)
        {
            return ExecuteWithErrorHandling<Exception>(action);
        }

        protected T ExecuteWithErrorHandling<T>(Action action)
            where T:Exception
        {
            try
            {
                action();
                return null;
            }
            catch (T ex)
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

        protected bool IsDefaultPassword()
        {
            try
            {
                return FormsAuthentication.Authenticate("admin", "changeme")
                    || System.Web.Security.Membership.ValidateUser("admin", "changeme");
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ShowTab(string tabName)
        {
            ClientScript.RegisterStartupScript(GetType(), "Content", "$(document).ready(function(){setTimeout(function(){$.fn.n2tabs_show($('#" + tabName + "'));}, 10);});", true);
        }
    }

    /// <summary>
    /// SqlCEDBHelper courtesy of Ayende Rahien from Rhino.Commons.Helpers
    /// Full code can be found here: https://svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/Helpers/SqlCEDbHelper.cs
    /// </summary>
    internal static class SqlCEDbHelper
    {
        static string engineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";

        internal static void CreateDatabaseFileAt(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            CreateDatabaseFile(string.Format("Data Source='{0}';", filename));
        }

        internal static void CreateDatabaseFile(string connectionString)
        {
            Type type = Type.GetType(engineTypeName);
            PropertyInfo localConnectionString = type.GetProperty("LocalConnectionString");
            MethodInfo createDatabase = type.GetMethod("CreateDatabase");
                        
            object engine = Activator.CreateInstance(type);
            localConnectionString
                .SetValue(engine, connectionString, null);
            createDatabase
                .Invoke(engine, new object[0]);
        }
    }
}
