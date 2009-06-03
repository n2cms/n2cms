using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.Web;
using N2.Web.UI.WebControls;
using N2.Web;
using System.Web.Security;
using N2.Security;

namespace N2.Edit
{
    [NavigationLinkPlugin("Edit", "edit", "../edit.aspx?selected={selected}", Targets.Preview, "~/edit/img/ico/page_edit.gif", 20, 
		GlobalResourceClassName = "Navigation")]
	[ToolbarPlugin("", "edit", "edit.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/page_edit.gif", 50, ToolTip = "edit", 
		GlobalResourceClassName = "Toolbar")]
	[ControlPanelLink("cpEdit", "~/edit/img/ico/page_edit.gif", "~/edit/edit.aspx?selected={Selected.Path}", "Edit page", 50, ControlPanelState.Visible)]
	[ControlPanelLink("cpEditPreview", "~/edit/img/ico/page_edit.gif", "~/edit/edit.aspx?selectedUrl={Selected.Url}", "Back to edit", 10, ControlPanelState.Previewing)]
	[ControlPanelPreviewPublish("Publish the currently displayed page version.", 20, 
		AuthorizedRoles = new string[] { "Administrators", "Editors", "admin" })]
	[ControlPanelPreviewDiscard("Irrecoverably delete the currently displayed version.", 30, 
		AuthorizedRoles = new string[] { "Administrators", "Editors", "admin" })]
	[ControlPanelEditingSave("Save changes", 10)]
	[ControlPanelLink("cpEditingCancel", "~/edit/img/ico/cancel.gif", "{Selected.Url}", "Cancel changes", 20, ControlPanelState.Editing, 
		UrlEncode = false)]
	public partial class Edit : EditPage
	{
		protected PlaceHolder phPluginArea;

		protected bool CreatingNew
		{
			get { return Request["discriminator"] != null; }
		}

		protected override void OnInit(EventArgs e)
		{
            if (Request["cancel"] == "reloadTop")
                hlCancel.NavigateUrl = "javascript:window.top.location.reload();";
            else
                hlCancel.NavigateUrl = CancelUrl();

			if(Request["refresh"] == "true")
				Refresh(SelectedItem, ToolbarArea.Navigation);

			InitPlugins();
			InitItemEditor();
			InitTitle();
			InitButtons();
			base.OnInit(e);
		}

		private void InitButtons()
		{
			btnSavePublish.Enabled = Engine.SecurityManager.IsAuthorized(User, SelectedItem, Permission.Publish);
			btnPreview.Enabled = Engine.SecurityManager.IsAuthorized(User, SelectedItem, Permission.Write);
			btnSaveUnpublished.Enabled = Engine.SecurityManager.IsAuthorized(User, SelectedItem, Permission.Write);
		}

		protected override void OnLoad(EventArgs e)
		{
			LoadZones();
			LoadInfo();

			if (!IsPostBack)
				RegisterSetupToolbarScript(SelectedItem);

			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			CheckRelatedVersions(ie.CurrentItem);
			
			base.OnPreRender(e);
		}



		protected void OnSaveCommand(object sender, CommandEventArgs e)
		{
			Validate();
			if (IsValid)
			{
				if (!Engine.SecurityManager.IsAuthorized(User, ie.CurrentItem, N2.Security.Permission.Publish))
				{
					FailValidation("Not authorized to publish.");
				}

				try
				{
					SaveChanges();
				}
				catch (Exception ex)
				{
					Engine.Resolve<IErrorHandler>().Notify(ex);
					FailValidation(ex.Message);
				}
			}
		}

    	void FailValidation(string message)
    	{
    		cvException.IsValid = false;
    		cvException.ErrorMessage = message;
    	}

    	protected void OnPreviewCommand(object sender, CommandEventArgs e)
		{
			Validate();
			if (IsValid)
			{
				ContentItem savedVersion = SaveVersion();

				Url redirectTo = Engine.EditManager.GetPreviewUrl(savedVersion);

				redirectTo = redirectTo.AppendQuery("preview", savedVersion.ID);
				if (savedVersion.VersionOf != null)
					redirectTo = redirectTo.AppendQuery("original", savedVersion.VersionOf.ID);
				if (!string.IsNullOrEmpty(Request["returnUrl"]))
					redirectTo = redirectTo.AppendQuery("returnUrl", Request["returnUrl"]);

				Response.Redirect(redirectTo);
			}
		}

		protected void OnSaveUnpublishedCommand(object sender, CommandEventArgs e)
		{
			Validate();
			if (IsValid)
			{
				ContentItem savedVersion = SaveVersion();
				Url redirectUrl = Engine.EditManager.GetEditExistingItemUrl(savedVersion);
				Response.Redirect(redirectUrl.AppendQuery("refresh=true"));
			}
		}
		
		
		
		protected override string GetToolbarSelectScript(ToolbarPluginAttribute toolbarPlugin)
		{
			if (CreatingNew)
				return "n2ctx.toolbarSelect('new');";

			return base.GetToolbarSelectScript(toolbarPlugin);
		}

		private void CheckRelatedVersions(ContentItem item)
		{
			hlNewerVersion.Visible = false;
			hlOlderVersion.Visible = false;

			if (item.VersionOf != null)
			{
				DisplayThisIsVersionInfo(item.VersionOf);
			}
			else
			{
				IList<ContentItem> unpublishedVersions = Find.Items
					.Where.VersionOf.Eq(item)
					.And.Updated.Gt(item.Updated)
					.OrderBy.Updated.Desc.MaxResults(1).Select();

				if(unpublishedVersions.Count > 0 && unpublishedVersions[0].Updated > item.Updated)
				{
					DisplayThisHasNewerVersionInfo(unpublishedVersions[0]);
				}
			}
		}

		private void DisplayThisHasNewerVersionInfo(ContentItem itemToLink)
		{
            string url = Url.ToAbsolute(Engine.EditManager.GetEditExistingItemUrl(itemToLink));
			hlNewerVersion.NavigateUrl = url;
			hlNewerVersion.Visible = true;
		}

		private void DisplayThisIsVersionInfo(ContentItem itemToLink)
		{
            string url = Url.ToAbsolute(Engine.EditManager.GetEditExistingItemUrl(itemToLink));
			hlOlderVersion.NavigateUrl = url;
			hlOlderVersion.Visible = true;
		}

		private void InitPlugins()
		{
			foreach(EditToolbarPluginAttribute plugin in Engine.EditManager.GetPlugins<EditToolbarPluginAttribute>(Page.User))
			{
				plugin.AddTo(phPluginArea, new PluginContext(SelectedItem, MemorizedItem, ControlPanelState.Visible));
			}
		}

		private void InitTitle()
		{
			if (ie.CurrentItem.ID == 0)
			{
				ItemDefinition definition = Engine.Definitions.GetDefinition(ie.CurrentItemType);
				string definitionTitle = GetGlobalResourceString("Definitions", definition.Discriminator + ".Title") ?? definition.Title;
				string format = GetLocalResourceString("EditPage.TitleFormat.New");
				Title = string.Format(format, definitionTitle);
			}
			else
			{
				string format = GetLocalResourceString("EditPage.TitleFormat.Update");
				Title = string.Format(format, ie.CurrentItem.Title);
			}
		}

		private void InitItemEditor()
		{
			string dataType = Request["dataType"];
			string discriminator = Request["discriminator"];
			if(!string.IsNullOrEmpty(discriminator))
			{
                ie.Discriminator = Engine.Definitions.GetDefinition(discriminator).Discriminator;
                ie.ParentPath = SelectedItem.Path;
			}
			else if (!string.IsNullOrEmpty(dataType))
			{
                Type t = Type.GetType(dataType);
                if (t == null)
                    throw new ArgumentException("Couldn't load a type of the given parameter '" + dataType + "'", "dataType");
                ItemDefinition d = Engine.Definitions.GetDefinition(discriminator);
                if(d == null)
                    throw new N2Exception("Couldn't find any definition for type '" + t + "'");
                ie.Discriminator = d.Discriminator;
				ie.ParentPath = SelectedItem.Path;
			}
			else
			{
				ie.CurrentItem = SelectedItem;
			}
			ie.ZoneName = base.Page.Request["zoneName"];
		}

		private void LoadZones()
		{
			Type itemType = ie.CurrentItemType;
			ucZones.CurrentItem = ie.CurrentItem;
			ItemDefinition definition = N2.Context.Definitions.GetDefinition(itemType);
			ucZones.DataSource = definition.AvailableZones;
			ucZones.DataBind();
		}

		private void LoadInfo()
		{
			ucInfo.CurrentItem = ie.CurrentItem;
			ucInfo.DataBind();
		}

        private void SaveChanges()
        {
			ItemEditorVersioningMode mode = (ie.CurrentItem.VersionOf == null) ? ItemEditorVersioningMode.VersionAndSave : ItemEditorVersioningMode.SaveAsMaster;
        	ContentItem currentItem = ie.Save(ie.CurrentItem, mode);

            if (Request["before"] != null)
            {
                ContentItem before = Engine.Resolve<N2.Edit.Navigator>().Navigate(Request["before"]);
                Engine.Resolve<ITreeSorter>().MoveTo(currentItem, NodePosition.Before, before);
            }
            else if (Request["after"] != null)
            {
                ContentItem after = Engine.Resolve<N2.Edit.Navigator>().Navigate(Request["after"]);
                Engine.Resolve<ITreeSorter>().MoveTo(currentItem, NodePosition.After, after);
            }

            Refresh(currentItem.VersionOf ?? currentItem, ToolbarArea.Both);
            Title = string.Format(GetLocalResourceString("SavedFormat"), currentItem.Title);
            ie.Visible = false;
        }

		private ContentItem SaveVersion()
		{
			ItemEditorVersioningMode mode = (ie.CurrentItem.VersionOf == null) ? ItemEditorVersioningMode.VersionOnly : ItemEditorVersioningMode.SaveOnly;
			return ie.Save(ie.CurrentItem, mode);
		}
	}
}