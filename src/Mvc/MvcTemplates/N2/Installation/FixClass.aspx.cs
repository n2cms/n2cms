using System;
using System.Collections.Generic;
using System.Data;
using N2.Configuration;
using N2.Edit.Installation;
using N2.Web;
using N2.Management.Installation;

namespace N2.Edit.Install
{
    public partial class FixClass : System.Web.UI.Page
    {
        NHInstallationManager installer = N2.Context.Current.Resolve<NHInstallationManager>();
        string tablePrefix = N2.Context.Current.Resolve<DatabaseSection>().TablePrefix;

        protected override void OnInit(EventArgs e)
        {
            InstallationUtility.CheckInstallationAllowed(Context);
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
	        if (String.IsNullOrEmpty(Request["id"]))
	        {
		        throw new ArgumentException("HTTP request parameter 'id' is required.");
	        }
	        int id;
	        if (!int.TryParse(Request["id"], out id))
	        {
		        throw new ArgumentException(String.Format("Failed to parse the given id '{0}' as an integer.", Request["id"]));
	        }

			Header.DataBind();

            if (!IsPostBack)
            {
                using (IDbConnection conn = installer.GetConnection())
                {
                    conn.Open();
                    string discriminator = GetDiscriminator(conn, id);
                    string itemsSql = string.Format("select * from {0}item where Type = '{1}'", tablePrefix, discriminator);
                    using (IDbCommand cmd = installer.GenerateCommand(CommandType.Text, itemsSql))
                    {
                        cmd.Connection = conn;
                        dgrItems.DataSource = cmd.ExecuteReader();
                    }

                    ddlType.DataSource = N2.Context.Definitions.GetDefinitions();
                    DataBind();
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs args)
        {
            using (IDbConnection conn = installer.GetConnection())
            {
                conn.Open();
                using (IDbCommand cmd = installer.GenerateCommand(CommandType.Text, ""))
                {
                    cmd.Connection = conn;

                    List<int> ids = new List<int>();

                    string discriminator = GetDiscriminator(conn, int.Parse(Request["id"]));
                    cmd.CommandText = string.Format("select id from {0}item where Type = '{1}'", tablePrefix, discriminator);
                    AppendIds(cmd, ids);
                    AppendChildrenIdsRecursive(cmd, ids);

                    ids.Reverse();
                    foreach (int id in ids)
                    {
                        cmd.CommandText = string.Format("delete from {0}detail where ItemID = ", tablePrefix) + id + " or LinkValue = " + id;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("delete from {0}detailcollection where ItemID = ", tablePrefix) + id;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("delete from {0}allowedrole where ItemID = ", tablePrefix) + id;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("delete from {0}item where ID = ", tablePrefix) + id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            lblResult.Text = "The items were deleted. Go back to the " + new Link("start page", "/") + " to see if it worked.";
        }

        private void AppendChildrenIdsRecursive(IDbCommand cmd, List<int> ids)
        {
            if (ids.Count == 0)
                return;

            List<int> newids = new List<int>();
            foreach (int id in ids)
            {
                cmd.CommandText = string.Format("select id from {0}item where ParentID = ", tablePrefix) + id;
                AppendIds(cmd, newids);
            }
            AppendChildrenIdsRecursive(cmd, newids);
            foreach (int id in newids)
            {
                if (!ids.Contains(id))
                    ids.Add(id);
            }
        }

        private static void AppendIds(IDbCommand cmd, List<int> ids)
        {
            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add(reader.GetInt32(0));
                }
            }
        }

        protected void btnChange_Click(object sender, EventArgs args)
        {
            using (IDbConnection conn = installer.GetConnection())
            {
                conn.Open();
                string discriminator = GetDiscriminator(conn, int.Parse(Request["id"]));
                string sql = string.Format("update {0}item set type = '{1}' where Type = '{2}'", tablePrefix, ddlType.SelectedValue.Replace("'", "''"), discriminator);
                using (IDbCommand cmd = installer.GenerateCommand(CommandType.Text, sql))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }

                lblResult.Text = "The items were updated. Go back to the " + new Link("start page", "/") + " to see if it worked.";
            }

        }

        private string GetDiscriminator(IDbConnection conn, int id)
        {
            string typeSql = string.Format("select Type from {0}item where ID = {1}", tablePrefix, id);
            using (IDbCommand cmd = installer.GenerateCommand(CommandType.Text, typeSql))
            {
                cmd.Connection = conn;
                return (string)cmd.ExecuteScalar();
            }
        }
    }
}
