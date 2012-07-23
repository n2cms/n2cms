using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.Web;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	[NavigationLinkPlugin("Edit", "edit", "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={selected}", Targets.Preview, "{ManagementUrl}/Resources/icons/page_edit.png", 20, 
		GlobalResourceClassName = "Navigation", 
		RequiredPermission = Permission.Write)]
	[ToolbarPlugin("EDIT", "edit", "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Preview, Targets.Preview, "{ManagementUrl}/Resources/icons/page_edit.png", 50, ToolTip = "edit",
		GlobalResourceClassName = "Toolbar", 
		RequiredPermission = Permission.Write)]
	[ControlPanelLink("cpEdit", "{ManagementUrl}/Resources/icons/page_edit.png", "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={Selected.Path}", "Edit page", 50, ControlPanelState.Visible, 
		RequiredPermission = Permission.Write)]
	[ControlPanelLink("cpEditPreview", "{ManagementUrl}/Resources/icons/page_edit.png", "{ManagementUrl}/Content/Edit.aspx?selectedUrl={Selected.Url}", "Back to edit", 10, ControlPanelState.Previewing, 
		RequiredPermission = Permission.Write)]
	[ControlPanelPreviewPublish("Publish the currently displayed page version.", 20, 
		RequiredPermission = Permission.Publish)]
	[ControlPanelPreviewDiscard("Irrecoverably delete the currently displayed version.", 30,
		RequiredPermission = Permission.Publish)]
	[ControlPanelEditingSave("Save changes", 10,
		RequiredPermission = Permission.Write)]
    [ControlPanelLink("cpEditingCancel", "{ManagementUrl}/Resources/icons/cancel.png", "{Selected.Url}", "Cancel changes", 20, ControlPanelState.Editing, 
		UrlEncode = false)]
	public partial class Edit : EditPage, IItemEditor
	{
		protected PlaceHolder phPluginArea;

		protected bool CreatingNew
		{
			get { return Request["discriminator"] != null; }
		}

		protected ISecurityManager Security;
		protected IDefinitionManager Definitions;
		protected IVersionManager Versions;
		protected CommandDispatcher Commands;
		protected IEditManager EditManager;
		protected IEditUrlManager ManagementPaths;

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			Security = Engine.SecurityManager;
			Definitions = Engine.Definitions;
			Versions = Engine.Resolve<IVersionManager>();
			Commands = Engine.Resolve<CommandDispatcher>();
			EditManager = Engine.EditManager;
			ManagementPaths = Engine.ManagementPaths;
		}

		protected override void OnInit(EventArgs e)
		{
			N2.Resources.Register.JQueryUi(this);

			if(Request["refresh"] == "true")
                Refresh(Selection.SelectedItem, ToolbarArea.Navigation);

			InitPlugins();
			InitItemEditor();
			InitTitle();
			InitButtons();

			base.OnInit(e);
		}

		private void InitButtons()
		{
            if (Request["cancel"] == "reloadTop")
                hlCancel.NavigateUrl = "javascript:window.top.location.reload();";
            else
                hlCancel.NavigateUrl = CancelUrl();

            bool isPublicableByUser = Security.IsAuthorized(User, ie.CurrentItem, Permission.Publish);
			bool isVersionable = Versions.IsVersionable(ie.CurrentItem);
            bool isWritableByUser = Security.IsAuthorized(User, Selection.SelectedItem, Permission.Write);
            bool isExisting = ie.CurrentItem.ID != 0;

            btnSavePublish.Visible = isPublicableByUser;
            btnPreview.Visible = isVersionable && isWritableByUser;
            btnSaveUnpublished.Visible = isVersionable && isWritableByUser;
			hlFuturePublish.Visible = isVersionable && isPublicableByUser;
		}

		protected override void OnLoad(EventArgs e)
		{
			LoadZones();
			LoadInfo();

			if (!IsPostBack)
                RegisterSetupToolbarScript(Selection.SelectedItem);

			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			CheckRelatedVersions(ie.CurrentItem);
			
			base.OnPreRender(e);
		}



        protected void OnPublishCommand(object sender, CommandEventArgs e)
		{
			var ctx = ie.CreateCommandContext();
			ctx.Parameters["MoveBefore"] = Request["before"];
			ctx.Parameters["MoveAfter"] = Request["after"];
			Commands.Publish(ctx);

			HandleResult(ctx, Request["returnUrl"], Engine.GetContentAdapter<NodeAdapter>(ctx.Content).GetPreviewUrl(ctx.Content));
		}

    	protected void OnPreviewCommand(object sender, CommandEventArgs e)
		{
			var ctx = ie.CreateCommandContext();
			Commands.Save(ctx);

			Url previewUrl = Engine.GetContentAdapter<NodeAdapter>(ctx.Content).GetPreviewUrl(ctx.Content);
			previewUrl = previewUrl.AppendQuery("preview", ctx.Content.ID);
			if(ctx.Content.VersionOf.HasValue)
				previewUrl = previewUrl.AppendQuery("original", ctx.Content.VersionOf.ID);

			HandleResult(ctx, previewUrl);
		}

		protected void OnSaveUnpublishedCommand(object sender, CommandEventArgs e)
		{
			var ctx = ie.CreateCommandContext();
            Commands.Save(ctx);

			Url redirectTo = ManagementPaths.GetEditExistingItemUrl(ctx.Content);
			if (!string.IsNullOrEmpty(Request["returnUrl"]))
				redirectTo = redirectTo.AppendQuery("returnUrl", Request["returnUrl"]);
			
			HandleResult(ctx, redirectTo);
        }

        protected void OnSaveFuturePublishCommand(object sender, CommandEventArgs e)
        {
            Validate();
            if (IsValid)
            {
                ContentItem savedVersion = SaveVersionForFuturePublishing();
				Url redirectUrl = ManagementPaths.GetEditExistingItemUrl(savedVersion);
				Response.Redirect(redirectUrl.AppendQuery("refresh=true"));
			}
        }




        private void HandleResult(CommandContext ctx, params string[] redirectSequence)
        {
            if (ctx.Errors.Count > 0)
            {
                string message = string.Empty;
                foreach (var ex in ctx.Errors)
                {
                    Engine.Resolve<IErrorNotifier>().Notify(ex);
                    message += ex.Message + "<br/>";
                }
                FailValidation(message);
            }
			else if(ctx.ValidationErrors.Count == 0)
			{
				string redirectUrl = redirectSequence.FirstOrDefault(u => !string.IsNullOrEmpty(u));

				if (ctx.RedirectUrl != null)
					Response.Redirect(ctx.RedirectUrl.ToUrl().AppendQuery("returnUrl", redirectUrl, unlessNull: true));

				Refresh(ctx.Content, redirectUrl ?? Engine.ManagementPaths.GetPreviewUrl(ctx.Content));
			}
        }

        void FailValidation(string message)
        {
            cvException.IsValid = false;
            cvException.ErrorMessage = message;
        }
		
		
		
		protected override string GetToolbarSelectScript(string toolbarPluginName)
		{
			if (CreatingNew)
				return "n2ctx.toolbarSelect('new');";

			return base.GetToolbarSelectScript(toolbarPluginName);
		}

		private void CheckRelatedVersions(ContentItem item)
		{
			hlNewerVersion.Visible = false;
			hlOlderVersion.Visible = false;

			if (item.VersionOf.HasValue)
			{
				DisplayThisIsVersionInfo(item.VersionOf);
			}
			else
			{
				IList<ContentItem> unpublishedVersions = Engine.Resolve<IItemFinder>()
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
            string url = Url.ToAbsolute(ManagementPaths.GetEditExistingItemUrl(itemToLink));
			hlNewerVersion.NavigateUrl = url;
			hlNewerVersion.Visible = true;
		}

		private void DisplayThisIsVersionInfo(ContentItem itemToLink)
		{
			string url = Url.ToAbsolute(ManagementPaths.GetEditExistingItemUrl(itemToLink));
			hlOlderVersion.NavigateUrl = url;
			hlOlderVersion.Visible = true;
		}

		private void InitPlugins()
		{
			var start = Engine.Resolve<IUrlParser>().StartPage;
			var root = Engine.Persister.Repository.Get(Engine.Resolve<IHost>().CurrentSite.RootItemID);
			foreach (EditToolbarPluginAttribute plugin in EditManager.GetPlugins<EditToolbarPluginAttribute>(Page.User))
			{
				plugin.AddTo(phPluginArea, new PluginContext(Selection, start, root,
					ControlPanelState.Visible, Engine, new HttpContextWrapper(Context)));
			}
		}

		private void InitTitle()
		{
			if (ie.CurrentItem.ID == 0)
			{
				ItemDefinition definition = Definitions.GetDefinition(ie.CurrentItemType);
				string definitionTitle = GetGlobalResourceString("Definitions", definition.Discriminator + ".Title") ?? definition.Title;
				string format = GetLocalResourceString("EditPage.TitleFormat.New", "New \"{0}\"");
				
				string template = Request["template"];
				if (!string.IsNullOrEmpty(template))
				{
					var info = Definitions.GetTemplate(definition.ItemType, template);
					definitionTitle = info.Title;
				}

				Title = string.Format(format, definitionTitle);
				
			}
			else
			{
				string format = GetLocalResourceString("EditPage.TitleFormat.Update", "Edit \"{0}\"");
				Title = string.Format(format, ie.CurrentItem.Title);
			}
		}

		private void InitItemEditor()
		{
			ie.AddPlaceHolder("Sidebar", phSidebar);

			string dataType = Request["dataType"];
			string discriminator = Request["discriminator"];
			string template = Request["template"];
			
			if (!string.IsNullOrEmpty(discriminator))
			{
				ie.Initialize(discriminator, template, Selection.SelectedItem);
			}
			else if (!string.IsNullOrEmpty(dataType))
			{
                Type t = Type.GetType(dataType);
                if (t == null)
                    throw new ArgumentException("Couldn't load a type of the given parameter '" + dataType + "'", "dataType");
                ItemDefinition d = Definitions.GetDefinition(discriminator);
                if(d == null)
                    throw new N2Exception("Couldn't find any definition for type '" + t + "'");
                ie.Discriminator = d.Discriminator;
				ie.ParentPath = Selection.SelectedItem.Path;
			}
			else
			{
				ie.Definition = Definitions.GetDefinition(Selection.SelectedItem);
                ie.CurrentItem = Selection.SelectedItem;
			}
			if (Request["zoneName"] != null)
			{
				ie.ZoneName = Request["zoneName"];
			}
            dpFuturePublishDate.SelectedDate = ie.CurrentItem.Published;
		}

		private void LoadZones()
		{
			ucZones.CurrentItem = ie.CurrentItem;
			ItemDefinition definition = Engine.Definitions.GetDefinition(ie.CurrentItem);
			ucZones.LoadZonesOf(definition, ie.CurrentItem);
		}

		private void LoadInfo()
		{
			ucInfo.CurrentItem = ie.CurrentItem;
			ucInfo.DataBind();
		}

        private ContentItem SaveVersionForFuturePublishing()
        {
            // Explicitly setting the current versions FuturePublishDate.
            // The database will end up with two new rows in the detail table.
            // On row pointing to the master and one to the latest/new version.
			var cc = ie.CreateCommandContext();
			Commands.Save(cc);

			var item = cc.Content;
			if (!item.VersionOf.HasValue)
				item.Published = dpFuturePublishDate.SelectedDate;
			else
				item["FuturePublishDate"] = dpFuturePublishDate.SelectedDate;
			Engine.Resolve<StateChanger>().ChangeTo(item, ContentState.Waiting);
			Engine.Persister.Save(item);
			return item;
        }

		#region IItemEditor Members

		public ItemEditorVersioningMode VersioningMode
		{
			get { return ie.VersioningMode; }
			set { ie.VersioningMode = value; }
		}

		public string ZoneName
		{
			get { return ie.ZoneName; }
			set { ie.ZoneName = value; }
		}

		public IDictionary<string, System.Web.UI.Control> AddedEditors
		{
			get { return ie.AddedEditors; }
		}

		public event EventHandler<ItemEventArgs> Saved
		{
			add { ie.Saved += value; }
			remove { ie.Saved -= value; }
		}

		#endregion

		#region IItemContainer Members

		public ContentItem CurrentItem
		{
			get { return ie.CurrentItem; }
		}

		#endregion
	}
}