#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Installation;
using N2.Web;
using N2.Engine;
using N2.Edit.Installation;

namespace N2.Edit.Install
{
	public partial class Diagnose : Page
	{
		protected IHost host;
		protected string[] recentChanges = new string[0];

		protected override void OnLoad(EventArgs e)
		{
			host = N2.Context.Current.Resolve<IHost>();

			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			ShowLastError();

			try
			{
				CheckDatabase();
			}
			catch (Exception ex)
			{
				lblDbConnection.Text = formatException(ex);
			}

			try
			{
				lblRootNode.Text = CurrentInstallationManager.CheckRootItem();
			}
			catch (Exception ex)
			{
				lblRootNode.Text = formatException(ex);
			}

			try
			{
				lblStartNode.Text = CurrentInstallationManager.CheckStartPage();
			}
			catch (Exception ex)
			{
				lblStartNode.Text = formatException(ex);
			}

			try
			{
				recentChanges = N2.Find.Items.All.MaxResults(10).OrderBy.Updated.Desc.Select().Select(i => i.ID + ": " + i.Title + " (" + i.Updated + ")").ToArray();
			}
			catch (Exception ex)
			{
				lblChanges.Text = formatException(ex);
			}

			try
			{
				lblN2Version.Text = (new AssemblyName(Assembly.Load("N2").FullName)).Version.ToString();
				lblEditVersion.Text = (new AssemblyName(Assembly.Load("N2.Management").FullName)).Version.ToString();
			}
			catch (Exception ex)
			{
				lblEditVersion.Text = formatException(ex);
			}

			try
			{
				rptDefinitions.DataSource = N2.Context.Definitions.GetDefinitions();
				rptDefinitions.DataBind();
			}
			catch (Exception ex)
			{
				lblDefinitions.Text = formatException(ex);
			}

            try
            {
                rptAssembly.DataSource = N2.Context.Current.Resolve<ITypeFinder>().GetAssemblies();
                rptAssembly.DataBind();
            }
            catch (Exception ex)
            {
                lblAssemblies.Text = formatException(ex);
            }
		}

		private void ShowLastError()
		{
			try
			{
				Exception lastError = Server.GetLastError();
				Server.ClearError();
				if (lastError != null)
					lblError.Text = formatException(lastError);
				else
					lblError.Text = "none";
			}
			catch (Exception ex)
			{
				lblError.Text = formatException(ex);
			}
		}

		private InstallationManager CurrentInstallationManager
		{
			get { return N2.Context.Current.Resolve<InstallationManager>(); }
		}

		private void CheckDatabase()
		{
			lblDbConnection.Text = CurrentInstallationManager.CheckDatabase();
		}
		protected void btnRestart_Click(object sender, EventArgs e)
		{
			HttpRuntime.UnloadAppDomain();
			Response.Redirect(Request.RawUrl);
		}
		protected void btnAddSchema_Click(object sender, EventArgs e)
		{
			try
			{
				CurrentInstallationManager.Install();
				lblAddSchemaResult.Text = "OK, tables created";
			}
			catch (Exception ex)
			{
				lblAddSchemaResult.Text = formatException(ex);
			}
		}

		protected void btnClearTables_Click(object sender, EventArgs e)
		{
			try
			{
				CurrentInstallationManager.DropDatabaseTables();
				lblClearTablesResult.Text = "OK, tables removed";
			}
			catch (Exception ex)
			{
				lblClearTablesResult.Text = formatException(ex);
			}
		}

		protected void btnInsert_Click(object sender, EventArgs e)
		{
			ddlTypes.Items.Clear();
			ddlTypes.Items.Add("[select an item destinationType to insert]");
			foreach (ItemDefinition d in N2.Context.Definitions.GetDefinitions())
				ddlTypes.Items.Add(new ListItem(d.Title, d.ItemType.FullName));
			ddlTypes.Visible = true;
			btnInsertRootNode.Visible = true;
		}

		protected void btnInsertRootNode_Click(object sender, EventArgs e)
		{
			try
			{
				ItemDefinition definition = GetSelectedRootNodeDefinition();
				int itemID =
					CurrentInstallationManager.InsertRootNode(definition.ItemType, "root", "Root node inserted by diagnostic page").ID;

				btnInsertRootNode.Visible = false;
				ddlTypes.Visible = false;


				if (itemID == N2.Context.Current.Resolve<IHost>().DefaultSite.RootItemID)
					lblInsert.Text = string.Format("Inserted root node with id {0} which matches root node in web.config. Great!", itemID);
				else
					lblInsert.Text = string.Format(
							"Inserted root node with id {0}. You should update web.config &lt;appSettings&gt;&lt;add name=\"N2.SiteRoot\" value=\"<b>{0}</b>\" /&gt;&lt;/appSettings&gt;",
							itemID);
			}
			catch (Exception ex)
			{
				lblInsert.Text = formatException(ex);
			}
		}

		private ItemDefinition GetSelectedRootNodeDefinition()
		{
			int i = 1;
			foreach (ItemDefinition definition in N2.Context.Definitions.GetDefinitions())
			{
				if (i++ == ddlTypes.SelectedIndex)
				{
					return definition;
				}
			}
			throw new N2Exception("Really bad, couldn't find the item type selected in the drop down list");
		}

		protected void ddlTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private string formatException(Exception ex)
		{
			return
				string.Format("<textarea>{0}\n\n{1}</textarea>", ex.Message, ex.StackTrace);
		}
	}
}
