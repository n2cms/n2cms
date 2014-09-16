using System;
using System.Configuration;
using N2.Configuration;
using N2.Management.Installation;

namespace N2.Edit.Install
{
    public partial class Fix : System.Web.UI.Page
    {
        string tablePrefix = N2.Context.Current.Resolve<DatabaseSection>().TablePrefix;
        
        protected override void OnInit(EventArgs e)
        {
            InstallationUtility.CheckInstallationAllowed(Context);

            rptCns.DataSource = ConfigurationManager.ConnectionStrings;
            rptCns.DataBind();

            string connectionStringName = Request.QueryString["cn"] ?? "N2CMS";
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            sdsItems.ConnectionString = settings.ConnectionString;
            //sdsItems.ProviderName = settings.ProviderName;

            //if (settings.ProviderName.StartsWith("System.Data.SqlServerCe"))
            //{
            //    gvItems.DataSourceID = "";
            //    gvItems.AllowPaging = false;
            //    gvItems.AllowSorting = false;
                
            //    using(var cmd = N2.Find.NH.Connection.CreateCommand())
            //    {
            //        cmd.CommandText = string.Format("SELECT * FROM [{0}Item]", tablePrefix);
            //        gvItems.DataSource = cmd.ExecuteReader();
            //    }
            //    gvItems.DataBind();
            //}

            if (!IsPostBack)
            {
                LoadItems();
            }
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            sdsItems.SelectCommand = string.Format("SELECT * FROM [{0}Item]", tablePrefix); 
            sdsItems.DeleteCommand = string.Format("DELETE FROM [{0}detail] WHERE [ItemID] = @ID DELETE FROM [{0}detailCollection] WHERE [ItemID] = @ID DELETE FROM [{0}AllowedRole] WHERE [ItemID] = @ID DELETE FROM [{0}Item] WHERE [ID] = @ID" , tablePrefix); 
            sdsItems.InsertCommand = string.Format("INSERT INTO [{0}Item] ([Type], [Updated], [Name], [ZoneName], [Title], [Created], [Published], [Expires], [SortOrder], [Visible], [SavedBy], [VersionOfID], [ParentID]) VALUES (@Type, @Updated, @Name, @ZoneName, @Title, @Created, @Published, @Expires, @SortOrder, @Visible, @SavedBy, @VersionOfID, @ParentID)" , tablePrefix);
            sdsItems.UpdateCommand = string.Format("UPDATE [{0}Item] SET [Type] = @Type, [Updated] = @Updated, [Name] = @Name, [ZoneName] = @ZoneName, [Title] = @Title, [Created] = @Created, [Published] = @Published, [Expires] = @Expires, [SortOrder] = @SortOrder, [Visible] = @Visible, [SavedBy] = @SavedBy, [VersionOfID] = @VersionOfID, [ParentID] = @ParentID WHERE [ID] = @ID", tablePrefix); 

			Header.DataBind();
        }

        private void LoadItems()
        {
            gvItems.DataSourceID = sdsItems.ID;
            try
            {
                lblError.Text = "";
                gvItems.DataBind();
            }
            catch (Exception ex)
            {
                gvItems.DataSourceID = "";
                lblError.Text = ex.Message;
            }
        }
    }
}
