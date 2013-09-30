﻿using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
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

		public bool HasChildren { get; set; }

		public bool Expanded { get; set; }
	}

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
	}

	public class InterfaceDefinition
	{
		public Node<InterfaceMenuItem> MainMenu { get; set; }

		public Node<InterfaceMenuItem> ActionMenu { get; set; }

		public Node<TreeNode> Content { get; set; }

		public Site Site { get; set; }

		public string Authority { get; set; }

		public InterfaceUser User { get; set; }

		public InterfacePaths Paths { get; set; }

		public Node<InterfaceMenuItem> ContextMenu { get; set; }

		public InterfacePartials Partials { get; set; }
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

	public class InterfaceUser
	{
		public string Name { get; set; }
		public string Username { get; set; }
		public string ViewPreference { get; set; }
	}

	//public class InterfaceTrash : Node<N2.Edit.TreeNode>
	//{
	//	public int TotalItems { get; set; }
	//	public int ChildItems { get; set; }
	//}

	public class InterfacePartials
	{
		public string Management { get; set; }

		public string Menu { get; set; }

		public string Preview { get; set; }

		public string Main { get; set; }

		public string Tree { get; set; }

		public string Search { get; set; }

		public string Footer { get; set; }

		public string ContextMenu { get; set; }
	}

	public class InterfaceBuiltEventArgs : EventArgs
	{
		public InterfaceDefinition Data { get; internal set; }
	}

	[Service]
	public class InterfaceBuilder
	{
		private IEngine engine;

		public InterfaceBuilder(IEngine engine)
		{
			this.engine = engine;
		}

		public event EventHandler<InterfaceBuiltEventArgs> InterfaceBuilt;

		public virtual InterfaceDefinition GetInterfaceDefinition(HttpContextBase context, SelectionUtility selection)
		{
			var data = new InterfaceDefinition
			{
				MainMenu = CreateMainMenu(),
				ActionMenu = CreateActionMenu(context),
				Content = CreateContent(context, selection),
				Site = engine.Host.GetSite(selection.SelectedItem),
				Authority = context.Request.Url.Authority,
				User = CreateUser(context),
				//Trash = CreateTrash(context),
				Paths = CreateUrls(context, selection),
				ContextMenu = CreateContextMenu(context),
				Partials = CreatePartials(context)
			};

			PostProcess(data);

			if (InterfaceBuilt != null)
				InterfaceBuilt(this, new InterfaceBuiltEventArgs { Data = data });

			return data;
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
				RemoveRemovedComponentsRecursive(children[i], removedComponents);

				if (children[i].Current == null || !removedComponents.Contains(children[i].Current.Name))
					continue;

				children.RemoveAt(i);
				node.Children = children.ToArray();
			}
		}

		private InterfacePartials CreatePartials(HttpContextBase context)
		{
			return new InterfacePartials
			{
				Management = "App/Partials/Management.html",
				Menu = "App/Partials/Menu.html",
				Main = "App/Partials/Main.html",
				Tree = "App/Partials/ContentTree.html",
				Preview = "App/Partials/ContentPreview.html",
				Search = "App/Partials/ContentSearch.html",
				Footer = "App/Partials/Footer.html",
				ContextMenu = "App/Partials/ContentContextMenu.html"
			};
		}

		protected virtual Node<InterfaceMenuItem> CreateContextMenu(HttpContextBase context)
		{
			var children = new List<Node<InterfaceMenuItem>> 
			{
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "add", Title = "Add", IconClass = "n2-icon-plus-sign", Target = Targets.Preview, Description = "Adds a new child items", Url = "{{appendSelection('{ManagementUrl}/Content/New.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Write }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "edit", Title = "Edit", IconClass = "n2-icon-edit-sign", Target = Targets.Preview, Description = "Edit details", Url = "{{appendSelection('{ManagementUrl}/Content/Edit.aspx', true)}}".ResolveUrlTokens(), RequiredPermission = Permission.Write }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "throw", Title = "Throw", IconClass = "n2-icon-trash", Url = "{{appendSelection('{ManagementUrl}/Content/Delete.aspx')}}".ResolveUrlTokens(), ToolTip = "Move selected item to trash", RequiredPermission = Permission.Publish, HiddenBy = "Deleted" }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "security", Title = "Manage security", IconClass = "n2-icon-lock", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Security/Default.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Administer }),
			};
			children.AddRange(engine.EditManager.GetPlugins<NavigationPluginAttribute>(context.User)
				.Where(np => !np.Legacy)
				.Select(np => CreateNode(np)));

			return new Node<InterfaceMenuItem>
			{
				Children = children
			};
		}

		protected virtual Node<InterfaceMenuItem> CreateNode(LinkPluginAttribute np)
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
					node.Children = options.GetAll().Select(o => CreateNode(o)).ToList();
				}
			}
			return node;
		}

		private Node<InterfaceMenuItem> CreateNode(ToolbarOption o)
		{
			return new Node<InterfaceMenuItem>(new InterfaceMenuItem
			{
				Title = o.Title,
				Name = o.Name,
				Target = o.Target,
				Url = Retoken(o.Url)
			});
		}

		static Dictionary<string, string> replacements = new Dictionary<string, string>
		{
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

		protected virtual InterfacePaths CreateUrls(HttpContextBase context, SelectionUtility selection)
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
				PreviewUrl = engine.GetContentAdapter<NodeAdapter>(selection.SelectedItem).GetPreviewUrl(selection.SelectedItem, allowDraft: true)
			};
		}

		protected virtual Node<InterfaceMenuItem> CreateActionMenu(HttpContextBase context)
		{
			var children = new List<Node<InterfaceMenuItem>>
			{
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "preview", ToolTip = "Fullscreen", Url = "{{Context.CurrentItem.PreviewUrl}}", Target = Targets.Top, IconClass = "n2-icon-eye-open", HiddenBy = "Management" })
				{
					Children = new Node<InterfaceMenuItem>[]
					{
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "fullscreen", Title = "Fullscreen", IconClass = "n2-icon-eye-open", Target = Targets.Top, Url = "{{Context.CurrentItem.PreviewUrl}}" }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "previewdivider1", Divider = true }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "viewdrafts", Title = "View latest drafts", IconClass = "n2-icon-circle-blank", 
							ClientAction = "setViewPreference('draft')",
							SelectedBy = "Viewdraft" }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "viewpublished", Title = "View published versions", IconClass = "n2-icon-play-sign", 
							ClientAction = "setViewPreference('published')",
							SelectedBy = "Viewpublished"  }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "previewdivider2", Divider = true }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "links", Title = "Show links", IconClass = "n2-icon-link", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/LinkTracker/Default.aspx')}}".ResolveUrlTokens() }),
					}
				},
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "add", TemplateUrl = "App/Partials/ContentAdd.html", RequiredPermission = Permission.Write, HiddenBy = "Management" }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "edit", Title = "Edit", IconClass = "n2-icon-edit-sign", Target = Targets.Preview, Description = "Page details", Url = "{{appendSelection('{ManagementUrl}/Content/Edit.aspx', true)}}".ResolveUrlTokens(), RequiredPermission = Permission.Write, HiddenBy = "Management" })
				{
					Children = new Node<InterfaceMenuItem>[]
					{
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "editdetails", Title = "Edit details", IconClass = "n2-icon-edit-sign", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Edit.aspx', true)}}".ResolveUrlTokens(), RequiredPermission = Permission.Write }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "organize", Title = "Organize parts", IconClass = "n2-icon-th-large", Target = Targets.Preview, Url = "{{appendQuery(Context.CurrentItem.PreviewUrl, 'edit=drag')}}", RequiredPermission = Permission.Write }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "divider5", Divider = true }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "security", Title = "Manage security", IconClass = "n2-icon-lock", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Security/Default.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Administer }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "bulk", Title = "Bulk editing", IconClass = "n2-icon-edit", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Export/BulkEditing.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Publish }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "export", Title = "Export", IconClass = "n2-icon-cloud-download", ToolTip = "Export content to file", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Export/Export.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Administer }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "import", Title = "Import", IconClass = "n2-icon-cloud-upload", ToolTip = "Import content from file", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Export/Default.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Administer }),
					}
				},
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "versions", TemplateUrl = "App/Partials/ContentVersions.html", Url = "{{appendSelection('{ManagementUrl}/Content/Versions/')}}".ResolveUrlTokens(), RequiredPermission = Permission.Publish, HiddenBy = "Management" }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "language", TemplateUrl = "App/Partials/ContentLanguage.html", Url = "{{appendSelection('{ManagementUrl}/Content/Globalization/')}}".ResolveUrlTokens(), RequiredPermission = Permission.Write, HiddenBy = "Management" }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "transitions", TemplateUrl = "App/Partials/ContentTransitions.html", RequiredPermission = Permission.Publish, HiddenBy = "Management" })
				{
					Children = new Node<InterfaceMenuItem>[]
					{
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "throw", Title = "Throw", IconClass = "n2-icon-trash", Url = "{{appendSelection('{ManagementUrl}/Content/Delete.aspx')}}".ResolveUrlTokens(), ToolTip = "Move selected item to trash", RequiredPermission = Permission.Publish, HiddenBy = "Deleted" }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "publish", Title = "Publish", IconClass = "n2-icon-play-sign", ClientAction = "publish()", RequiredPermission = Permission.Publish, HiddenBy = "Published" }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "schedule", TemplateUrl = "App/Partials/ContentPublishSchedule.html", RequiredPermission = Permission.Publish, DisplayedBy = "Draft" }),
						new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "unpublish", Title = "Unpublish", IconClass = "n2-icon-stop", ClientAction = "unpublish()", RequiredPermission = Permission.Publish, DisplayedBy = "Published" }),
					}
				},
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "frameaction", TemplateUrl = "App/Partials/FrameAction.html", RequiredPermission = Permission.Write }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "close", Title = "Close", Url = "{{Context.ReturnUrl || Context.CurrentItem.PreviewUrl || Context.Paths.PreviewUrl}}", Target = Targets.Preview, DisplayedBy = "Management", HiddenBy = "Unclosable" }),
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "info", TemplateUrl = "App/Partials/ContentInfo.html", RequiredPermission = Permission.Read, HiddenBy = "Management", Alignment = "Right" })
			};

			children.AddRange(engine.EditManager.GetPlugins<ToolbarPluginAttribute>(context.User)
					.Where(np => !np.Legacy)
					.Select(np => CreateNode(np)));

			return new Node<InterfaceMenuItem>
			{
				Children = children
			};
		}

		//protected virtual InterfaceTrash CreateTrash(HttpContextBase context)
		//{
		//	var trash = engine.Resolve<ITrashHandler>();

		//	if (trash.TrashContainer == null)
		//		return new InterfaceTrash();

		//	var container = (ContentItem)trash.TrashContainer;

		//	var total = (int)engine.Persister.Repository.Count(Parameter.Below(container));
		//	var children = (int)engine.Persister.Repository.Count(Parameter.Equal("Parent", container));

		//	return new InterfaceTrash
		//	{
		//		Current = engine.GetContentAdapter<NodeAdapter>(container).GetTreeNode(container),
		//		HasChildren = children > 0,
		//		ChildItems = children,
		//		TotalItems = total,
		//		Children = new Node<TreeNode>[0]
		//	};
		//}

		protected virtual InterfaceUser CreateUser(HttpContextBase context)
		{
			return new InterfaceUser
			{
				Name = context.User.Identity.Name,
				Username = context.User.Identity.Name,
				ViewPreference = context.GetViewPreference(engine.Config.Sections.Management.Versions.DefaultViewMode).ToString().ToLower()
			};
		}

		protected virtual Node<TreeNode> CreateContent(HttpContextBase context, SelectionUtility selection)
		{
			var filter = engine.EditManager.GetEditorFilter(context.User);

			var structure = new BranchHierarchyBuilder(selection.SelectedItem, selection.Traverse.RootPage, true) { UseMasterVersion = false }
				.Children((item) =>
				{
					var q = new N2.Persistence.Sources.Query { Parent = item, OnlyPages = true, Interface = Interfaces.Managing, Filter = filter };
					return engine.GetContentAdapter<NodeAdapter>(item).GetChildren(q);
				})
				.Build();

			return CreateStructure(structure, filter);
		}

		protected virtual Node<TreeNode> CreateStructure(HierarchyNode<ContentItem> structure, ItemFilter filter)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(structure.Current);

			var children = structure.Children.Select(c => CreateStructure(c, filter)).ToList();
			return new Node<TreeNode>
			{
				Current = adapter.GetTreeNode(structure.Current),
				HasChildren = adapter.HasChildren(structure.Current, filter),
				Expanded = children.Any(),
				Children = children
			};
		}

		protected virtual Node<InterfaceMenuItem> CreateMainMenu()
		{
			return new Node<InterfaceMenuItem>
			{
				Children = new Node<InterfaceMenuItem>[]
				{
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "dashboard", Title = "Dashboard", IconClass = "n2-icon-home" , Target = Targets.Preview, Url = engine.Content.Traverse.RootPage.Url, SelectedBy = "MyselfRoot" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "pages", Title = "Pages", IconClass = "n2-icon-edit", Target = "_top", Url = "{ManagementUrl}".ResolveUrlTokens(), SelectedBy = "ContentPages" }),
					
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "divider1", Divider = true }),
					
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "sitesettings", Title = "Site Settings", IconClass = "n2-icon-cog", ToolTip = "Edit site settings", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/EditRecursive.aspx')}}".ResolveUrlTokens(), RequiredPermission = Permission.Write, SelectedBy = "ContentEditRecursive" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "templates", Title = "Templates", IconClass = "n2-icon-plus-sign-alt", ToolTip = "Show predefined templates with content", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Templates/Default.aspx')}}".ResolveUrlTokens().ResolveUrlTokens(), RequiredPermission = Permission.Administer, SelectedBy = "ContentTemplatesDefault" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "wizards", Title = "Wizards", IconClass = "n2-icon-magic", ToolTip = "Show predefined types and locations for content", Target = Targets.Preview, Url = "{{appendSelection('{ManagementUrl}/Content/Wizard/Default.aspx')}}".ResolveUrlTokens(), SelectedBy = "ContentWizardDefault" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "users", Title = "Users", IconClass = "n2-icon-user", ToolTip = "Manage users", Target = Targets.Preview, Url = "{ManagementUrl}/Users/Users.aspx".ResolveUrlTokens(), RequiredPermission = Permission.Administer, SelectedBy = "UsersUsers" }),
					
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "divider2", Divider = true }),

					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "signout", Title = "Sign out", IconClass = "n2-icon-signout", ToolTip = "Sign out {{Context.User.Name}}", Url = "{ManagementUrl}/Login.aspx?logout=true".ResolveUrlTokens() }),
				}
			};
		}
	}
}
