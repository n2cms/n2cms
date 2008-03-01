using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Installation;

namespace N2.Edit.Install
{
	public partial class _Default : Page
	{
		protected FileUpload fileUpload;

		private InstallationManager currentInstallationManager;
		private DatabaseStatus status;

		protected int rootId;
		protected int startId;
	
		public InstallationManager CurrentInstallationManager
		{
			get
			{ 
				if (currentInstallationManager == null)
					currentInstallationManager = new InstallationManager(N2.Context.Current);
				return currentInstallationManager; 
			}
		}

		public DatabaseStatus Status
		{
			get
			{
				if (status == null) 
					status = CurrentInstallationManager.GetStatus();
				return status;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				try
				{
					ICollection<ItemDefinition> definitions = N2.Context.Definitions.GetDefinitions();
					ICollection<ItemDefinition> rootDefinitions = new List<ItemDefinition>();
					ICollection<ItemDefinition> startPageDefinitions = new List<ItemDefinition>();

					foreach (ItemDefinition d in definitions)
					{
						if(d.DefinitionAttribute.MayBeRoot)
							rootDefinitions.Add(d);
						if(d.DefinitionAttribute.MayBeStartPage)
							startPageDefinitions.Add(d);
					}

					if (rootDefinitions.Count == 0)
						rootDefinitions = definitions;
					if (startPageDefinitions.Count == 0)
						startPageDefinitions = definitions;

					ddlRoot.Items.Clear();
					ddlRoot.Items.Add("[select an item type to insert as root node]");
					foreach (ItemDefinition d in rootDefinitions)
					{
						ddlRoot.Items.Add(new ListItem(d.Title, d.ItemType.AssemblyQualifiedName));
					}

					ddlStartPage.Items.Clear();
					ddlStartPage.Items.Add("[use the root as start page]");
					foreach (ItemDefinition d in startPageDefinitions)
					{
						ddlStartPage.Items.Add(new ListItem(d.Title, d.ItemType.AssemblyQualifiedName));
					}
				}
				catch (Exception ex)
				{
					ltStartupError.Text = "<li style='color:red'>Ooops, something is wrong: " + ex.Message + "</li>";
					return;
				}
			}
		}

		protected void btnTest_Click(object sender, EventArgs e)
		{
			try
			{
				InstallationManager im = CurrentInstallationManager;

				using (IDbConnection conn = im.GetConnection())
				{
					conn.Open();
					lblStatus.CssClass = "ok";
					lblStatus.Text = "Connection OK";
				}
			}
			catch (Exception ex)
			{
				lblStatus.CssClass = "warning";
				lblStatus.Text = "Connection problem, hopefully this error message can help you figure out what's wrong: <br/>" +
				                 ex.Message;
				lblStatus.ToolTip = ex.StackTrace;
			}
		}

		protected void btnInstall_Click(object sender, EventArgs e)
		{
			InstallationManager im = CurrentInstallationManager;
			if (Request.QueryString["export"] == "true")
			{
				im.ExportSchema(Response.Output);
				Response.End();
			}
			else
			{
				im.Install();
				lblInstall.Text = "Database created, now insert root items.";
			}
		}

		protected void btnInsert_Click(object sender, EventArgs e)
		{
			cvRoot.Validate();
			if(!cvRoot.IsValid)
				return;
			
			InstallationManager im = CurrentInstallationManager;

			try
			{
				cvRoot.IsValid = ddlRoot.SelectedIndex > 0;
				if (cvRoot.IsValid)
				{
					ContentItem root = im.InsertRootNode(Type.GetType(ddlRoot.SelectedValue), "root", "Root node");

					if (root.ID == Status.RootItemID)
					{
						ltRootNode.Text = "<span class='ok'>Root node inserted.</span>";
						phSame.Visible = false;
						phDiffer.Visible = false;
                        rootId = root.ID;
                    }
					else
					{
						ltRootNode.Text = string.Format(
							"<span class='warning'>Root node inserted but you must update web.config with root item id: <b>{0}</b></span> ", 
							root.ID);
						phSame.Visible = true;
						phDiffer.Visible = false;
						rootId = root.ID;
					}

					if (ddlStartPage.SelectedIndex > 0)
					{
						ContentItem startPage = im.InsertStartPage(Type.GetType(ddlStartPage.SelectedValue), root, "start", "Start page");
						if (startPage.ID == Status.StartPageID && root.ID == Status.RootItemID)
						{
							ltRootNode.Text = "<span class='ok'>Root and start page inserted.</span>";
						}
						else
						{
							ltRootNode.Text = string.Format(
								"<span class='warning'>Start page inserted but you must update web.config with root item id: <b>{0}</b> and start page id: <b>{1}</b></span>", root.ID, startPage.ID);
							phSame.Visible = false;
							phDiffer.Visible = true;
							startId = startPage.ID;
                        }
					}

				}
			}
			catch (Exception ex)
			{
				ltRootNode.Text = string.Format("<span class='warning'>{0}</span><!--\n{1}\n-->", ex.Message, ex);
			}
		}

		protected void btnUpload_Click(object sender, EventArgs e)
		{
			Validate();
			if(!IsValid)
				return;

			try
			{
				InstallFromUpload();
			}
			catch (Exception ex)
			{
				ltRootNode.Text = string.Format("<span class='warning'>{0}</span>", ex.Message);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			DataBind();
		}

		protected string GetStatusText()
		{
            if (Status.Installation)
			{
             	return "You're all set (just check step 5).";
            }
			else if (Status.Schema)
			{
            	return "Jump to step 4.";
            }
			else if (Status.Connection) 
			{
            	return "Skip to step 3.";
            }
			else
			{
            	return "Continue to step 2.";
            }
		}

		private void InstallFromUpload()
		{
			InstallationManager im = CurrentInstallationManager;
			ContentItem root = im.InsertExportFile(fileUpload.FileContent, fileUpload.FileName);

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
				rootId = root.ID;
			}
			foreach(ContentItem item in root.Children)
			{
				ItemDefinition id = N2.Context.Definitions.GetDefinition(item.GetType());
				if(id.DefinitionAttribute.MayBeStartPage)
				{
					if (item.ID == Status.StartPageID && root.ID == Status.RootItemID)
					{
						ltRootNode.Text = "<span class='ok'>Root and start page inserted.</span>";
					}
					else
					{
						ltRootNode.Text = string.Format(
							"<span class='warning'>Start page inserted but you must update web.config with root item id: <b>{0}</b> and start page id: <b>{1}</b></span>", root.ID, item.ID);
						phSame.Visible = false;
						phDiffer.Visible = true;
						startId = item.ID;
					}
					break;
				}
			}
		}
	}
}