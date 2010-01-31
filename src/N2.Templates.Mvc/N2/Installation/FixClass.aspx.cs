using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using N2.Installation;
using N2.Web;
using N2.Edit.Installation;

namespace N2.Edit.Install
{
    public partial class FixClass : System.Web.UI.Page
    {
        InstallationManager installer = N2.Context.Current.Resolve<InstallationManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (IDbConnection conn = installer.GetConnection())
                {
                    conn.Open();
                    string sql = "select * from n2item where Type = (select Type from n2item where ID = " + int.Parse(Request["id"]) + ")";
                    using (IDbCommand cmd = installer.GenerateCommand(CommandType.Text, sql))
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

                    cmd.CommandText = "select id from n2item where Type = (select Type from n2item where ID = " + int.Parse(Request["id"]) + ")";
                    AppendIds(cmd, ids);
                    AppendChildrenIdsRecursive(cmd, ids);

                    ids.Reverse();
                    foreach(int id in ids)
                    {                        
                        cmd.CommandText = "delete from n2detail where ItemID = " + id + " or LinkValue = " + id;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "delete from n2allowedrole where ItemID = " + id;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "delete from n2item where ID = " + id;
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
                cmd.CommandText = "select id from n2item where ParentID = " + id;
                AppendIds(cmd, newids);
            }
            AppendChildrenIdsRecursive(cmd, newids);
            foreach (int id in newids)
            {
                if(!ids.Contains(id))
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
                string sql = "update n2item set type = '" + ddlType.SelectedValue.Replace("'", "''") + "' where Type = (select Type from n2item where ID = " + int.Parse(Request["id"]) + ")";
                using (IDbCommand cmd = installer.GenerateCommand(CommandType.Text, sql))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }

                lblResult.Text = "The items were updated. Go back to the " + new Link("start page", "/") + " to see if it worked.";
            }

        }
    }
}
