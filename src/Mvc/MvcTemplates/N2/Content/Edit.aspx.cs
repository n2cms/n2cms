using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.Versioning;
using N2.Edit.Web;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Edit.Activity;
using N2.Management.Activity;
using N2.Edit.AutoPublish;

namespace N2.Edit
{
	[NavigationLinkPlugin("Edit", "edit", "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={selected}", Targets.Preview, "{ManagementUrl}/Resources/icons/page_edit.png", 20,
		GlobalResourceClassName = "Navigation",
		RequiredPermission = Permission.Write,
		IconClass = "fa fa-pencil-square",
		Legacy = true)]
	[ToolbarPlugin("EDIT", "edit", "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Preview, Targets.Preview, "{ManagementUrl}/Resources/icons/page_edit.png", 50, ToolTip = "edit",
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Write,
		OptionProvider = typeof(EditOptionProvider),
		Legacy = true)]
	[ControlPanelLink("cpEdit", "{ManagementUrl}/Resources/icons/page_edit.png", "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={Selected.Path}&n2versionIndex={Selected.VersionIndex}", "Edit page", 50, ControlPanelState.Visible | ControlPanelState.DragDrop,
		CssClass = "complementary",
		RequiredPermission = Permission.Write,
		IconClass = "fa fa-pencil-square",
		Legacy = true)]
	[ControlPanelPreviewPublish("Publish draft", 70,
		RequiredPermission = Permission.Publish)]
	[ControlPanelEditingSave("Save changes", 10,
		RequiredPermission = Permission.Write)]
	[ControlPanelLink("cpEditingCancel", "{ManagementUrl}/Resources/icons/cancel.png", "{Selected.Url}", "Cancel changes", 20, ControlPanelState.Editing,
		UrlEncode = false,
		IconClass = "fa fa-check-minus")]
	[N2.Management.Activity.ActivityNotification]
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
		protected ContentVersionRepository Repository;

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			Security = Engine.SecurityManager;
			Definitions = Engine.Definitions;
			Versions = Engine.Resolve<IVersionManager>();
			Commands = Engine.Resolve<CommandDispatcher>();
			EditManager = Engine.EditManager;
			ManagementPaths = Engine.ManagementPaths;
			Repository = Engine.Resolve<ContentVersionRepository>();
		}

		protected override void OnInit(EventArgs e)
		{
			N2.Resources.Register.JQueryUi(this);

			if (Request["refresh"] == "true")
				Refresh(Selection.SelectedItem, ToolbarArea.Navigation);

			InitPlugins();
			InitItemEditor();
			InitTitle();
			InitButtons();

			base.OnInit(e);
		}

		private void InitButtons()
		{
			bool isPublicableByUser = Security.IsAuthorized(User, ie.CurrentItem, Permission.Publish);
			bool isPublicableItem = ie.CurrentItem.IsPage || !ie.CurrentItem.IsVersionable();
			bool isVersionable = Versions.IsVersionable(ie.CurrentItem);
			bool isWritableByUser = Security.IsAuthorized(User, Selection.SelectedItem, Permission.Write);
			bool isExisting = ie.CurrentItem.ID != 0;

			btnSavePublish.Visible = isPublicableItem && isPublicableByUser;
			btnPreview.Visible = isVersionable && isWritableByUser;
			btnSaveUnpublished.Visible = isVersionable && isWritableByUser;
			hlFuturePublish.Visible = isVersionable && isPublicableByUser;
			btnUnpublish.Visible = isExisting && isPublicableByUser && ie.CurrentItem.IsPublished();
		}

		protected override void OnLoad(EventArgs e)
		{
			LoadZones();
			LoadInfo();
			LoadActivity();
			LoadVersions();

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
			ApplySortInfo(ctx);
			Commands.Publish(ctx);

			Engine.AddActivity(new ManagementActivity { Operation = "Publish", PerformedBy = User.Identity.Name, Path = ie.CurrentItem.Path, ID = ie.CurrentItem.ID });

			HandleResult(ctx, Request["returnUrl"], Engine.GetContentAdapter<NodeAdapter>(ctx.Content).GetPreviewUrl(ctx.Content));
		}

		private void ApplySortInfo(CommandContext ctx)
		{
			ctx.Parameters["MoveBefore"] = Request["before"];
			ctx.Parameters["MoveBeforeVersionKey"] = Request["beforeVersionKey"];
			ctx.Parameters["BelowVersionKey"] = Request["belowVersionKey"];
			ctx.Parameters["MoveAfter"] = Request["after"];
			ctx.Parameters["MoveBeforeSortOrder"] = Request["beforeSortOrder"];
		}

		protected void OnPreviewCommand(object sender, CommandEventArgs e)
		{
			var ctx = ie.CreateCommandContext();
			ApplySortInfo(ctx);
			Commands.Save(ctx);

			var page = Find.ClosestPage(ctx.Content);
			Url previewUrl = Engine.GetContentAdapter<NodeAdapter>(page).GetPreviewUrl(page);
			if (Request["edit"] == "drag")
				previewUrl = previewUrl.SetQueryParameter("edit", "drag");

			Engine.AddActivity(new ManagementActivity { Operation = "Preview", PerformedBy = User.Identity.Name, Path = ie.CurrentItem.Path, ID = ie.CurrentItem.ID });

			HandleResult(ctx, Request["returnUrl"], previewUrl);
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

		protected void OnUnpublishCommand(object sender, CommandEventArgs e)
		{
			var item = ie.CurrentItem;
			item.State = ContentState.Unpublished;
			Engine.Persister.Save(item);
			Refresh(item, ToolbarArea.Both);
		}

		protected void OnSaveFuturePublishCommand(object sender, CommandEventArgs e)
		{
			Validate();
			if (IsValid)
			{
				ContentItem savedVersion = SaveVersionForFuturePublishing();

				Engine.AddActivity(new ManagementActivity { Operation = "FuturePublish", PerformedBy = User.Identity.Name, Path = ie.CurrentItem.Path, ID = ie.CurrentItem.ID });

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
			else if (ctx.ValidationErrors.Count == 0)
			{
				string redirectUrl = redirectSequence.FirstOrDefault(u => !string.IsNullOrEmpty(u));

				if (ctx.RedirectUrl != null)
					Response.Redirect(ctx.RedirectUrl.ToUrl().AppendQuery("returnUrl", redirectUrl, unlessNull: true));

				Refresh(ctx.Content, ToolbarArea.Navigation);
				var previewUrl = (redirectUrl ?? Engine.GetContentAdapter<NodeAdapter>(ctx.Content).GetPreviewUrl(ctx.Content)).ToUrl();
				previewUrl = previewUrl.SetQueryParameter("refresh", "true");
				previewUrl = previewUrl.SetQueryParameter("n2scroll", Request["n2scroll"]);
				if (!string.IsNullOrEmpty(Request["n2reveal"]))
					previewUrl = previewUrl.SetQueryParameter("n2reveal", Request["n2reveal"]);
				else if (!ctx.Content.IsPage)
					previewUrl = previewUrl.SetQueryParameter("n2reveal", "part" + (string.IsNullOrEmpty(ctx.Content.GetVersionKey()) ? ctx.Content.ID.ToString() : ctx.Content.GetVersionKey()));

				Refresh(ctx.Content, previewUrl);
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

			if (!item.IsPage)
				return;

			if (item.VersionOf.HasValue)
			{
				DisplayThisIsVersionInfo(item.VersionOf);
			}
			else
			{
				var page = Find.ClosestPage(item);
				var version = Engine.Resolve<N2.Edit.Versioning.DraftRepository>().FindDrafts(page).FirstOrDefault();
				if (version != null && version.Saved > item.Updated)
				{
					DisplayThisHasNewerVersionInfo(Repository.DeserializeVersion(version).FindPartVersion(item));
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
			ItemDefinition definition = Definitions.GetDefinition(ie.CurrentItemType);
			string definitionTitle = GetGlobalResourceString("Definitions", definition.Discriminator + ".Title") ?? definition.Title;

			if (ie.CurrentItem.ID == 0 && !ie.CurrentItem.VersionOf.HasValue)
			{
				string format = GetLocalResourceString("EditPage.TitleFormat.New", "New \"{0}\"");

				string template = Request["template"];
				if (!string.IsNullOrEmpty(template))
				{
					var info = Engine.Resolve<ITemplateAggregator>().GetTemplate(definition.ItemType, template);
					definitionTitle = info.Title;
				}

				Title = string.Format(format, definitionTitle);
			}
			else
			{
				string format = GetLocalResourceString("EditPage.TitleFormat.Update", "Edit \"{0}\"");
				Title = string.Format(format, string.IsNullOrEmpty(ie.CurrentItem.Title) ? definitionTitle : ie.CurrentItem.Title);
			}
			Items["HelpText"] = definition.HelpText;
			Items["EditingInstructions"] = definition.EditingInstructions;
        }

		private void InitItemEditor()
		{
			ie.AddPlaceHolder("Sidebar", phSidebar);

			var request = new HttpRequestWrapper(Request);
			
			string dataType = EditExtensions.GetDataType(request);
			string discriminator = EditExtensions.GetDiscriminator(request);
			string template = EditExtensions.GetTemplate(request);

			if (!string.IsNullOrEmpty(discriminator))
			{
				ie.Initialize(discriminator, template, Selection.SelectedItem);
			}
			else if (!string.IsNullOrEmpty(dataType))
			{
				Type t = Type.GetType(dataType);
				if (t == null)
					throw new ArgumentException(String.Format("Couldn't load a type of the given parameter '{0}'", dataType), "dataType");
				ItemDefinition d = Definitions.GetDefinition(discriminator);
				if (d == null)
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

		private void LoadActivity()
		{
			ucActivity.CurrentItem = ie.CurrentItem;
			ucActivity.DataBind();
		}

		private void LoadVersions()
		{
			ucVersions.CurrentItem = ie.CurrentItem;
			ucVersions.DataBind();
		}

		private ContentItem SaveVersionForFuturePublishing()
		{
			// Explicitly setting the current versions FuturePublishDate.
			// The database will end up with two new rows in the detail table.
			// On row pointing to the master and one to the latest/new version.
			var cc = ie.CreateCommandContext();
			Commands.Save(cc);

			if (dpFuturePublishDate.SelectedDate.HasValue)
			{
				Engine.Resolve<PublishScheduledAction>().MarkForFuturePublishing(cc.Content, dpFuturePublishDate.SelectedDate.Value);
				Engine.Persister.Save(cc.Content);
			}
			return cc.Content;
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

		public event Action<object, CommandContext> CreatingContext
		{
			add { ie.CreatingContext += value; }
			remove { ie.CreatingContext -= value; }
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
