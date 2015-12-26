using System.Web.Security;
using N2.Collections;
using N2.Edit;
using N2.Engine;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Diagnostics;
using N2.Plugin;
using System.IO;
using N2.Web.Parts;
using N2.Edit.Api;
using N2.Edit.Versioning;

namespace N2.Management.Api
{
	[DebuggerDisplay("Node: {Current}")]
	public class Node<T>
    {
        public Node()
        {
            Children = new Node<T>[0];
        }

        public Node(T current)
            : this()
        {
            Current = current;
        }

        public T Current { get; set; }

        public IEnumerable<Node<T>> Children { get; set; }

		public int Count
		{
			get { return Children != null ? Children.Count() : 0; }
		}

        public bool HasChildren { get; set; }

        public bool Expanded { get; set; }
    }

	[DebuggerDisplay("InterfaceMenuItem: {Name}")]
    public class InterfaceMenuItem
    {
        public InterfaceMenuItem()
        {
            Target = Targets.Preview;
            RequiredPermission = Permission.Read;
        }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Target { get; set; }

        public string IconClass { get; set; }

        public string Description { get; set; }

        public string ToolTip { get; set; }

        public string IconUrl { get; set; }

        public string Name { get; set; }

        public bool IsDivider { get; set; }

        public string TemplateUrl { get; set; }

        public Permission RequiredPermission { get; set; }

        public string HiddenBy { get; set; }

        public string DisplayedBy { get; set; }

        public string SelectedBy { get; set; }

        public string Alignment { get; set; }

        public string ClientAction { get; set; }

        public bool Divider { get; set; }

		public static implicit operator Node<InterfaceMenuItem>(InterfaceMenuItem item)
		{
			return new Node<InterfaceMenuItem>(item);
		}

		public override string ToString()
		{
			return "InterfaceMenuItem " + Name;
		}
    }

    public class InterfaceDefinition
    {
        public Node<InterfaceMenuItem> MainMenu { get; set; }

        public Node<InterfaceMenuItem> ActionMenu { get; set; }

        public Node<TreeNode> Content { get; set; }

        public Site Site { get; set; }

        public string Authority { get; set; }

        public ProfileUser User { get; set; }

        public InterfacePaths Paths { get; set; }

        public Node<InterfaceMenuItem> ContextMenu { get; set; }

        public InterfacePartials Partials { get; set; }
    }

	public class ControlPanelDefinition
	{
		public TreeNode CurrentItem { get; set; }
		public Node<InterfaceMenuItem> Menu { get; set; }
        public InterfacePaths Paths { get; set; }
        public ActivityTrackingConfiguration ActivityTracking { get; set; }
		public List<TemplateInfo> Templates { get; internal set; }
		public DraftInfo Draft { get; internal set; }
		public string[] Dependencies { get; internal set; }
	}
	public class ActivityTrackingConfiguration
	{
		public int Interval { get; internal set; }
		public string Path { get; internal set; }
	}
	public class InterfacePaths
    {
        public string Management { get; set; }

        public string Create { get; set; }

        public string Delete { get; set; }

        public string Edit { get; set; }

        public string SelectedQueryKey { get; set; }

        public string ItemQueryKey { get; set; }

        public string PreviewUrl { get; set; }

        public string PageQueryKey { get; set; }
	}

    public class InterfacePartials
    {
        public string Management { get; set; }

        public string Menu { get; set; }

        public string Preview { get; set; }

        public string Main { get; set; }

        public string Tree { get; set; }
		
        public string Footer { get; set; }

        public string ContextMenu { get; set; }
    }

    public class InterfaceBuiltEventArgs : InterfaceventArgs<InterfaceDefinition>
    {
    }

	public class InterfaceventArgs<T> : EventArgs
	{
		public T Data { get; internal set; }
	}

	[Service]
    public class InterfaceBuilder
    {
        private readonly IEngine engine;

        public InterfaceBuilder(IEngine engine)
        {
            this.engine = engine;
        }

        public event EventHandler<InterfaceBuiltEventArgs> InterfaceBuilt;

        public event EventHandler<InterfaceventArgs<ControlPanelDefinition>> ControlPanelBuilt;

		public virtual InterfaceDefinition GetInterfaceDefinition(HttpContextBase context, SelectionUtility selection)
		{
			var profile = GetUser(context);
			var selectedItem = GetSelectedItem(selection, profile);

			var data = new InterfaceDefinition
			{
				MainMenu = GetMainMenu(),
				ActionMenu = GetActionMenu(context),
				ContextMenu = GetContextMenu(context),
				Content = GetContent(context, selection, selectedItem),
				Site = engine.Host.GetSite(selectedItem),
				Authority = context.Request.Url.Authority,
				User = profile,
				Paths = GetUrls(context, selectedItem),
				Partials = GetPartials(context)
			};

			PostProcess(data);

			if (InterfaceBuilt != null)
				InterfaceBuilt(this, new InterfaceBuiltEventArgs { Data = data });

			return data;
		}

		public virtual ControlPanelDefinition GetControlPanelDefinition(HttpContextBase context, ContentItem item)
		{
			var state = Web.UI.WebControls.ControlPanel.GetState(engine);
			var templates = new List<TemplateInfo>();
			if (state.IsFlagSet(Web.UI.WebControls.ControlPanelState.DragDrop))
			{
				var a = engine.ResolveAdapter<PartsAdapter>(item);
				foreach (var d in Web.UI.WebControls.ControlPanel.GetPartDefinitions(a, item, null, context.User))
				{
					foreach (var t in a.GetTemplates(item, d))
					{
						templates.Add(new TemplateInfo(t) { EditUrl = engine.ManagementPaths.GetEditNewPageUrl(item, t.Definition) });
					}
				}
			}

			var angularRoot = engine.Config.Sections.Web.Resources.AngularJsRoot;
            var data = new ControlPanelDefinition
			{
				Menu = GetControlPanelMenu(context, item),
				Paths = GetUrls(context, item),
				CurrentItem = engine.GetContentAdapter<NodeAdapter>(item).GetTreeNode(item),
				ActivityTracking = new ActivityTrackingConfiguration
				{
					Interval = engine.Config.Sections.Management.Collaboration.PingInterval,
					Path = engine.Config.Sections.Management.Collaboration.ActivityTrackingEnabled ? engine.Config.Sections.Management.Collaboration.PingPath.ResolveUrlTokens() : null
				},
				Templates = templates,
				Draft = engine.Resolve<DraftRepository>().GetDraftInfo(item),
				Dependencies = new[]
				{
					angularRoot + "angular.js",
					angularRoot + "angular-resource.js",
					"{ManagementUrl}/App/i18n/en.js.ashx",
					"{ManagementUrl}/App/Js/Directives.js",
					"{ManagementUrl}/App/Js/Services.js",
					"{ManagementUrl}/App/Preview/Preview.js"
				}.Select(p => p.ResolveUrlTokens()).ToArray()
			};
			
			PostProcess(data);

			if (ControlPanelBuilt != null)
				ControlPanelBuilt(this, new InterfaceventArgs<ControlPanelDefinition> { Data = data });

			return data;
		}

		private Node<InterfaceMenuItem> GetControlPanelMenu(HttpContextBase context, ContentItem item)
		{
			var state = N2.Web.UI.WebControls.ControlPanel.GetState(engine);
			var children = new List<Node<InterfaceMenuItem>>();
			children.AddRange(engine.EditManager.GetPlugins<ControlPanelLinkAttribute>(context.User)
				.Where(np => !np.Legacy)
				.Where(np => np.ShowDuring.HasFlag(state))
				.Select(np => GetNode(context, np, item, state)));
			return new Node<InterfaceMenuItem> { Children = children };
		}

		protected virtual void PostProcess(ControlPanelDefinition data)
		{
		}

		protected virtual void PostProcess(InterfaceDefinition data)
        {
            var removedComponents = new HashSet<string>(engine.Config.Sections.Engine.InterfacePlugins.RemovedElements.Select(re => re.Name));
            if (removedComponents.Count == 0)
                return;

            RemoveRemovedComponentsRecursive(data.MainMenu, removedComponents);
            RemoveRemovedComponentsRecursive(data.ActionMenu, removedComponents);
            RemoveRemovedComponentsRecursive(data.ContextMenu, removedComponents);
        }

        private void RemoveRemovedComponentsRecursive(Node<InterfaceMenuItem> node, HashSet<string> removedComponents)
        {
            if (node.Children == null)
                return;

            var children = node.Children.ToList();
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i] != null)
                {
                    RemoveRemovedComponentsRecursive(children[i], removedComponents);

                    if (children[i].Current == null || !removedComponents.Contains(children[i].Current.Name))
                        continue;
                }
                children.RemoveAt(i);
            }
            node.Children = children.ToArray();
        }

        private InterfacePartials GetPartials(HttpContextBase context)
        {
            return new InterfacePartials
            {
                Management = "App/Partials/Management.html",
                Menu = "App/Partials/Menu.html",
                Main = "App/Partials/Main.html",
                Tree = "App/Partials/ContentTree.html",
                Preview = "App/Partials/ContentPreview.html",
                Footer = "App/Partials/Footer.html",
                ContextMenu = "App/Partials/ContentContextMenu.html"
            };
        }

        protected virtual Node<InterfaceMenuItem> GetContextMenu(HttpContextBase context)
        {
			string deleteItemUrl = engine.Config.Sections.Management.Paths.DeleteItemUrl.ResolveUrlTokens();
	        string editItemUrl = engine.Config.Sections.Management.Paths.EditItemUrl.ResolveUrlTokens();
	        string newItemUrl = engine.Config.Sections.Management.Paths.NewItemUrl.ResolveUrlTokens();
	        string setSecurityUrl = engine.Config.Sections.Management.Paths.SetSecurityUrl.ResolveUrlTokens();

			var children = new List<Node<InterfaceMenuItem>> 
            {
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "add", Title = "Add", IconClass = "fa fa-plus-circle", Target = Targets.Preview, Description = "Adds a new child items", Url = "{{ContextMenu.appendSelection('" + newItemUrl + "')}}", RequiredPermission = Permission.Write }),
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "edit", Title = "Edit", IconClass = "fa fa-pencil-square", Target = Targets.Preview, Description = "Edit details", Url = "{{ContextMenu.appendSelection('" + editItemUrl + "', 'displayed')}}", RequiredPermission = Permission.Write }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "organize", Title = "Organize parts", IconClass = "fa fa-object-group", Target = Targets.Preview, Description = "Drag and drop edit mode", Url = "{{appendQuery(ContextMenu.CurrentItem.PreviewUrl, 'edit=drag')}}", RequiredPermission = Permission.Write }),
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "delete", Title = "Delete", IconClass = "fa fa-trash-o", Url = "{{ContextMenu.appendSelection('" + deleteItemUrl + "')}}", ToolTip = "Move selected item to trash", RequiredPermission = Permission.Publish, HiddenBy = "Deleted" }),
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "security", Title = "Manage security", IconClass = "fa fa-lock", Target = Targets.Preview, Url = "{{ContextMenu.appendSelection('" + setSecurityUrl + "')}}", RequiredPermission = Permission.Administer }),
            };
            children.AddRange(engine.EditManager.GetPlugins<NavigationPluginAttribute>(context.User)
                .Where(np => !np.Legacy)
                .Select(np => GetNode(np)));

            return new Node<InterfaceMenuItem>
            {
                Children = children
            };
        }

		private Node<InterfaceMenuItem> GetNode(HttpContextBase context, ControlPanelLinkAttribute np, ContentItem item, Web.UI.WebControls.ControlPanelState state)
		{
            var node = new Node<InterfaceMenuItem>(new InterfaceMenuItem
			{
				Title = np.Title,
				Name = np.Name,
				Target = np.Target,
				ToolTip = np.ToolTip,
				IconUrl = string.IsNullOrEmpty(np.IconClass) ? Retoken(np.IconUrl) : null,
				Url = Retoken(np.NavigateUrl),
				IsDivider = np is ControlPanelSeparatorAttribute,
				IconClass = np.IconClass
			});
			return node;
		}

		protected virtual Node<InterfaceMenuItem> GetNode(LinkPluginAttribute np)
        {
            var node = new Node<InterfaceMenuItem>(new InterfaceMenuItem
            {
                Title = np.Title,
                Name = np.Name,
                Target = np.Target,
                ToolTip = np.ToolTip,
                IconUrl = string.IsNullOrEmpty(np.IconClass) ? Retoken(np.IconUrl) : null,
                Url = Retoken(np.UrlFormat),
                IsDivider = np.IsDivider,
                IconClass = np.IconClass
            });
            if (np is ToolbarPluginAttribute)
            {
                var tp = np as ToolbarPluginAttribute;
                if (tp.OptionProvider != null)
                {
                    var options = (IProvider<ToolbarOption>)engine.Resolve(tp.OptionProvider);
                    node.Children = options.GetAll().Select(o => GetNode(o)).ToList();
                }
            }
            return node;
        }

        private Node<InterfaceMenuItem> GetNode(ToolbarOption o)
        {
            return new Node<InterfaceMenuItem>(new InterfaceMenuItem
            {
                Title = o.Title,
                Name = o.Name,
                Target = o.Target,
                Url = Retoken(o.Url)
            });
        }

        static readonly Dictionary<string, string> replacements = new Dictionary<string, string>
        {
             { "{Selected.Url}", "{{Context.CurrentItem.Url}}" },
             { "{Selected.Path}", "{{Context.CurrentItem.Path}}" },
             { "{Selected.VersionIndex}", "{{Context.CurrentItem.VersionIndex}}" },
			 { "{selected}", "{{Context.CurrentItem.Path}}" },
			 { "{memory}", "{{Context.Memory.Path}}" },
             { "{action}", "{{Context.Memory.Action}}" },
             { "{ManagementUrl}", Url.ResolveTokens("{ManagementUrl}") },
             { "{Selection.SelectedQueryKey}", Url.ResolveTokens("{SelectedQueryKey}") }
        };

        private string Retoken(string urlFormat)
        {
            if (string.IsNullOrEmpty(urlFormat))
                return urlFormat;

            foreach (var kvp in replacements)
                urlFormat = urlFormat.Replace(kvp.Key, kvp.Value);
            return urlFormat;
        }

        protected virtual InterfacePaths GetUrls(HttpContextBase context, ContentItem selectedItem)
        {

            return new InterfacePaths
            {
                Management = engine.ManagementPaths.GetManagementInterfaceUrl(),
                Delete = engine.Config.Sections.Management.Paths.DeleteItemUrl.ResolveUrlTokens(),
                Edit = engine.Config.Sections.Management.Paths.EditItemUrl.ResolveUrlTokens(),
                SelectedQueryKey = PathData.SelectedQueryKey,
                ItemQueryKey = PathData.ItemQueryKey,
                PageQueryKey = PathData.PageQueryKey,
                Create = engine.Config.Sections.Management.Paths.NewItemUrl.ResolveUrlTokens(),
                PreviewUrl = engine.GetContentAdapter<NodeAdapter>(selectedItem).GetPreviewUrl(selectedItem, allowDraft: true)
            };
        }

        protected virtual Node<InterfaceMenuItem> GetActionMenu(HttpContextBase context)
        {
            var children = new []
            {
                GetPreviewMenu(),
                GetCreateMenu(),
                GetEditMenu(),
                GetVersionsMenu(),
                GeLanguageMenu(),
                GetTransitionsMenu(),
                GetActionMenu(),
				GetMessagesMenu(),
                GetSearchMenu(),
                GetUserMenu(context.User),
                GetInfoMenu()
            }.Where(n => n != null).ToList();

            children.AddRange(engine.EditManager.GetPlugins<ToolbarPluginAttribute>(context.User)
                    .Where(np => !np.Legacy)
                    .Select(np => GetNode(np)));

            return new Node<InterfaceMenuItem>
            {
                Children = children
            };
        }

		protected virtual Node<InterfaceMenuItem> GetPreviewMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "preview", ToolTip = "Fullscreen", Url = "{{Context.CurrentItem.PreviewUrl}}", Target = Targets.Top, IconClass = "fa fa-eye" })
			{
				Children = new[]
                    {
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "fullscreen", Title = "Fullscreen", IconClass = "fa fa-arrows-alt", Target = Targets.Top, Url = "{{Context.CurrentItem.PreviewUrl}}" }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "previewdivider1", Divider = true }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "viewdrafts", Title = "View latest drafts", IconClass = "fa fa-circle-o", 
                            ClientAction = "setViewPreference('draft')",
                            SelectedBy = "Viewdraft" }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "viewpublished", Title = "View published versions", IconClass = "fa fa-play-circle", 
                            ClientAction = "setViewPreference('published')",
                            SelectedBy = "Viewpublished"  }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "previewdivider2", Divider = true }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "links", Title = "Show links", IconClass = "fa fa-link", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/LinkTracker/Default.aspx')}}".ResolveUrlTokens() })
                    }
			};
		}

		protected virtual Node<InterfaceMenuItem> GetCreateMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "add", TemplateUrl = "App/Partials/ContentAdd.html", RequiredPermission = Permission.Write });
		}

		protected virtual Node<InterfaceMenuItem> GetEditMenu()
		{
			string editItemUrl = engine.Config.Sections.Management.Paths.EditItemUrl.ResolveUrlTokens();
			string setSecurityUrl = engine.Config.Sections.Management.Paths.SetSecurityUrl.ResolveUrlTokens();

			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "edit", Title = "Edit", TemplateUrl = "App/Partials/MenuNodeLastChild.html", RequiredPermission = Permission.Write })
			{
				Children = new[]
                    {
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "organize", Title = "Organize parts", IconClass = "fa fa-object-group", Target = Targets.Preview, Url = "{{appendQuery(Context.CurrentItem.PreviewUrl, 'edit=drag')}}", RequiredPermission = Permission.Write }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "editdetails", Title = "Properties", IconClass = "fa fa-pencil-square", Target = Targets.Preview, Url = "{{appendSelection('" + editItemUrl + "', true)}}", RequiredPermission = Permission.Write }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "divider5", Divider = true }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "security", Title = "Manage security", IconClass = "fa fa-lock", Target = Targets.Preview, Url = "{{appendSelection('" + setSecurityUrl + "')}}", RequiredPermission = Permission.Administer }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "bulk", Title = "Bulk editing", IconClass = "fa fa-edit", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Export/BulkEditing.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Publish }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "export", Title = "Export", IconClass = "fa fa-cloud-download", ToolTip = "Export content to file", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Export/Export.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Administer }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "import", Title = "Import", IconClass = "fa fa-cloud-upload", ToolTip = "Import content from file", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Export/Default.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Administer })
                    }
			};
		}

		protected virtual Node<InterfaceMenuItem> GetVersionsMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "versions", TemplateUrl = "App/Partials/ContentVersions.html", Url = "{{appendSelection('{ManagementUrl}/Content/Versions/')}}".ResolveUrlTokens(), RequiredPermission = Permission.Publish });
		}

		protected virtual Node<InterfaceMenuItem> GeLanguageMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "language", TemplateUrl = "App/Partials/ContentLanguage.html", Url = "{{appendSelection('{ManagementUrl}/Content/Globalization/')}}".ResolveUrlTokens(), RequiredPermission = Permission.Write });
		}

		protected virtual Node<InterfaceMenuItem> GetTransitionsMenu()
		{
			string deleteItemUrl = engine.Config.Sections.Management.Paths.DeleteItemUrl.ResolveUrlTokens();

			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "transitions", TemplateUrl = "App/Partials/ContentTransitions.html", RequiredPermission = Permission.Publish })
			{
				Children = new[]
                    {
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "delete", Title = "Delete", IconClass = "fa fa-trash-o", Url = "{{appendSelection('" + deleteItemUrl + "')}}", ToolTip = "Move selected item to trash", RequiredPermission = Permission.Publish, HiddenBy = "Deleted" }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "publish", Title = "Publish", IconClass = "fa fa-play-sign", ClientAction = "publish()", RequiredPermission = Permission.Publish, HiddenBy = "Published" }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "schedule", TemplateUrl = "App/Partials/ContentPublishSchedule.html", RequiredPermission = Permission.Publish, DisplayedBy = "Draft" }),
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "unpublish", Title = "Unpublish", IconClass = "fa fa-stop", ClientAction = "unpublish()", RequiredPermission = Permission.Publish, DisplayedBy = "Published" }),
                    }
			};
		}

		protected virtual Node<InterfaceMenuItem> GetActionMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "frameaction", TemplateUrl = "App/Partials/FrameAction.html", RequiredPermission = Permission.Write });
		}

		protected virtual Node<InterfaceMenuItem> GetMessagesMenu()
		{
			return new InterfaceMenuItem { Name = "messages", Alignment = "Right", TemplateUrl = "App/Partials/Messages.html" };
		}

		protected virtual Node<InterfaceMenuItem> GetSearchMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "search", Alignment = "Right", TemplateUrl = "App/Partials/ContentSearch.html" });
		}

		protected virtual Node<InterfaceMenuItem> GetUserMenu(IPrincipal user)
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "me", Url = engine.Content.Traverse.RootPage.Url, ToolTip = user.Identity.Name, Alignment = "Right", IconClass = "fa fa-user" })
			{
				Children = new[]
                    {
                        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "password", Title = "Change password", IconClass = "fa fa-user", ToolTip = "Manage password", Target = Targets.Preview, Url = "{Account.EditPassword.PageUrl}".ResolveUrlTokens(), SelectedBy = "EditPassword" }),
  				        new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "signout", Title = "Sign out", IconClass = "fa fa-signout", ToolTip = "Sign out {{Context.User.Name}}", Url = "{Account.Logout.PageUrl}".ResolveUrlTokens() }),
                        /*
                        REMOVE: new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "password", Title = "Change password", IconClass = "fa fa-user", ToolTip = "Manage password", Target = Targets.Preview, Url = "{ManagementUrl}/Myself/EditPassword.aspx".ResolveUrlTokens(), SelectedBy = "EditPassword" }),
                        REMOVE: new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "signout", Title = "Sign out", IconClass = "fa fa-signout", ToolTip = "Sign out {{Context.User.Name}}", Url = "{ManagementUrl}/Login.aspx?logout=true".ResolveUrlTokens() }),
                         */
                    }
			};
		}

		protected virtual Node<InterfaceMenuItem> GetInfoMenu()
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "info", TemplateUrl = "App/Partials/ContentInfo.html", RequiredPermission = Permission.Read, Alignment = "Right" })
			{
				Children = new[]
                {
                    new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "infodetails", TemplateUrl = "App/Partials/ContentInfoDetails.html" })
				}
			};
		}

        protected virtual ProfileUser GetUser(HttpContextBase context)
        {
            var profile = engine.Resolve<IProfileRepository>().GetOrCreate(context.User);
            if (!profile.Settings.ContainsKey("ViewPreference"))
                profile.Settings["ViewPreference"] = context.GetViewPreference(engine.Config.Sections.Management.Versions.DefaultViewMode).ToString().ToLower();

            return profile;
        }

        protected virtual Node<TreeNode> GetContent(HttpContextBase context, SelectionUtility selection, ContentItem selectedItem)
        {
            var filter = engine.EditManager.GetEditorFilter(context.User) & Content.Is.Page();

            var root = selection.Traverse.RootPage;
            var structure = ApiExtensions.BuildBranchStructure(filter, engine.Resolve<IContentAdapterProvider>(), selectedItem, root);

            return GetStructure(structure, filter);
        }

        private static ContentItem GetSelectedItem(SelectionUtility selection, ProfileUser profile)
        {
            var selectedItem = selection.ParseSelectionFromRequest();
            if (selectedItem == null && profile.Settings.ContainsKey("Selected"))
                selectedItem = selection.ParseSelected((string)profile.Settings["Selected"]);
            return selectedItem ?? (selection.Traverse.StartPage);
        }

        protected virtual Node<TreeNode> GetStructure(HierarchyNode<ContentItem> structure, ItemFilter filter)
        {
            return structure.CreateNode(engine.Resolve<IContentAdapterProvider>(), filter);
        }

        protected virtual Node<InterfaceMenuItem> GetMainMenu()
        {
            var items = new List<Node<InterfaceMenuItem>>
            {
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "dashboard", Title = "Dashboard", IconClass = "fa fa-home" , Target = Targets.Preview, Url = engine.Content.Traverse.RootPage.Url, SelectedBy = "MyselfRoot" }),           
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "pages", Title = "Pages", IconClass = "fa fa-edit", Target = "_top", Url = "{ManagementUrl}".ResolveUrlTokens(), SelectedBy = "ContentPages" }),                   
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "divider1", Divider = true }),
                    
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "sitesettings", Title = "Site Settings", IconClass = "fa fa-cog", ToolTip = "Edit site settings", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/EditRecursive.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Write, SelectedBy = "ContentEditRecursive" }),
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "templates", Title = "Templates", IconClass = "fa fa-plus-square-o", ToolTip = "Show predefined templates with content", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Templates/Default.aspx')}}".ResolveUrlTokens().ResolveUrlTokens(), RequiredPermission = Permission.Administer, SelectedBy = "ContentTemplatesDefault" }),
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "wizards", Title = "Wizards", IconClass = "fa fa-magic", ToolTip = "Show predefined types and locations for content", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Wizard/Default.aspx')}}".ResolveUrlTokens(), SelectedBy = "ContentWizardDefault" }),
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "divider2", Divider = true }),
        
                new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "users", Title = "Users", IconClass = "fa fa-user shadow", ToolTip = "Manage users", Target = Targets.Preview, Url = "{Account.Users.PageUrl}".ResolveUrlTokens(), RequiredPermission = Permission.Administer, SelectedBy = "UsersUsers" }),
            };
            if (Roles.Enabled)
                items.Add (new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "roles", Title = "Roles", IconClass = "fa fa-group", ToolTip = "Manage roles", Target = Targets.Preview, Url = "{Account.Roles.PageUrl}".ResolveUrlTokens(), RequiredPermission = Permission.Administer, SelectedBy = "RolesList" }));

            return new Node<InterfaceMenuItem> { Children = items.ToArray() };
        }
    }
}
