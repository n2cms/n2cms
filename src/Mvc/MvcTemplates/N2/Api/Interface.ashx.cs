using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

		public string Alignment { get; set; }

		public string ClientAction { get; set; }
	}

	public class InterfaceData
	{
		public Node<InterfaceMenuItem> MainMenu { get; set; }
		
		public Node<InterfaceMenuItem> ToolbarMenu { get; set; }

		public Node<InterfaceMenuItem> ActionMenu { get; set; }

		public Node<TreeNode> Content { get; set; }

		public Site Site { get; set; }

		public string Authority { get; set; }

		public InterfaceUser User { get; set; }

		public InterfaceTrash Trash { get; set; }

		public InterfacePaths Paths { get; set; }

		public Node<InterfaceMenuItem> ContextMenu { get; set; }
	}

	public class InterfacePaths
	{
		public string Management { get; set; }

		public string Create { get; set; }

		public string Delete { get; set; }

		public string Edit { get; set; }

		public string SelectedQueryKey { get; set; }

		public string ViewPreference { get; set; }

		public string PreviewUrl { get; set; }
	}

	public class InterfaceUser
	{
		public string Name { get; set; }
		public string Username { get; set; }
		public ViewPreference PreferredView { get; set; }
	}

	public class InterfaceTrash : Node<TreeNode>
	{
		public int TotalItems { get; set; }
		public int ChildItems { get; set; }
	}

	public class Interface : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";
			new InterfaceData
			{
				MainMenu = CreateMainMenu(),
				ActionMenu = CreateActionMenu(context),
				Content = CreateContent(context),
				Site = engine.Host.GetSite(selection.SelectedItem),
				Authority = context.Request.Url.Authority,
				User = CreateUser(context),
				Trash = CreateTrash(context),
				Paths = CreateUrls(context),
				ContextMenu = CreateContextMenu(context)
			}.ToJson(context.Response.Output);
		}

		private Node<InterfaceMenuItem> CreateContextMenu(HttpContext context)
		{
			return new Node<InterfaceMenuItem>
			{
				Children = engine.EditManager.GetPlugins<NavigationPluginAttribute>(context.User)
					.Where(np => !np.Legacy)
					.Select(np => new Node<InterfaceMenuItem>(new InterfaceMenuItem 
					{
						Title = np.Title,
						Name = np.Name, 
						Target = np.Target, 
						ToolTip = np.ToolTip,
						IconUrl = Retoken(np.IconUrl), 
						Url = Retoken(np.UrlFormat),
						IsDivider = np.IsDivider
					})).ToList()
			};
		}

		static Dictionary<string, string> replacements = new Dictionary<string, string>
		{
			 { "{selected}", "{{Context.CurrentItem.Path}}" },
			 { "{memory}", "{{Context.Memory.Path}}" },
			 { "{action}", "{{Context.Memory.Action}}" },
			 { "{ManagementUrl}", Url.ResolveTokens("{ManagementUrl}") },
			 { "{Selection.SelectedQueryKey}", "{{Interface.Paths.SelectedQueryKey}}" }
		};

		private string Retoken(string urlFormat)
		{
			if (string.IsNullOrEmpty(urlFormat))
				return urlFormat;

			foreach (var kvp in replacements)
				urlFormat = urlFormat.Replace(kvp.Key, kvp.Value);
			return urlFormat;
		}

		private InterfacePaths CreateUrls(HttpContext context)
		{
			
			return new InterfacePaths {
				Management = engine.ManagementPaths.GetManagementInterfaceUrl(),
				Delete = engine.Config.Sections.Management.Paths.DeleteItemUrl.ResolveUrlTokens(),
				Edit = engine.Config.Sections.Management.Paths.EditItemUrl.ResolveUrlTokens(),
				SelectedQueryKey = engine.Config.Sections.Management.Paths.SelectedQueryKey.ResolveUrlTokens(),
				Create = engine.Config.Sections.Management.Paths.NewItemUrl.ResolveUrlTokens(),
				ViewPreference = new HttpContextWrapper(context).GetViewPreference(engine.Config.Sections.Management.Versions.DefaultViewMode).ToString(),
				PreviewUrl = engine.GetContentAdapter<NodeAdapter>(selection.SelectedItem).GetPreviewUrl(selection.SelectedItem)
			};
		}

		private Node<InterfaceMenuItem> CreateActionMenu(HttpContext context)
		{
			string mgmt = "{ManagementUrl}".ResolveUrlTokens();

			return new Node<InterfaceMenuItem>
			{
				Children = new Node<InterfaceMenuItem>[]
				{
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Url = "{{Context.CurrentItem.PreviewUrl}}", Target = Targets.Preview, IconClass = "icon-eye-open" })
					{
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Fullscreen", IconClass = "icon-fullscreen", Target = Targets.Top, Url = "{{Context.CurrentItem.PreviewUrl}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "View latest drafts", IconClass = "icon-fast-forward", Target = Targets.Top, Url = mgmt + "/?view=draft&{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "View published versions", IconClass = "icon-off", Target = Targets.Top, Url = mgmt + "/?view=published&{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Show links", IconClass = "icon-link", Target = Targets.Preview, Url = mgmt + "/Content/LinkTracker/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}" }),
						}
					},
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/PageAdd.html", RequiredPermission = Permission.Write, HiddenBy = "Management" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { IconClass = "icon-trash", Url = mgmt + "/Content/Delete.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}", ToolTip = "Throw selected item", RequiredPermission = Permission.Publish, HiddenBy = "Management" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Edit", IconClass = "icon-edit-sign", Target = Targets.Preview, Description = "Page details", Url = "{{Interface.Paths.Edit}}?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}&versionIndex={{Context.CurrentItem.VersionIndex}}", RequiredPermission = Permission.Write, HiddenBy = "Management" })
					{
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Edit {{Context.CurrentItem.Title}}", IconClass = "icon-edit", Target = Targets.Preview, Url = "{{Interface.Paths.Edit}}?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}&versionIndex={{Context.CurrentItem.VersionIndex}}", RequiredPermission = Permission.Write }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Organize parts", IconClass = "icon-th-large", Target = Targets.Preview, Url = "{{Context.CurrentItem.PreviewUrl}}&edit=drag", RequiredPermission = Permission.Write }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Manage security", IconClass = "icon-lock", Target = Targets.Preview, Url = mgmt + "/Content/Security/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}", RequiredPermission = Permission.Administer }),
						}
					},
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/PageVersions.html", Url = mgmt + "/Content/Versions/?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}", RequiredPermission = Permission.Publish, HiddenBy = "Management" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/PageLanguage.html", Url = mgmt + "/Content/Globalization/?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&item={{Context.CurrentItem.ID}}", RequiredPermission = Permission.Write, HiddenBy = "Management" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/PagePublish.html", RequiredPermission = Permission.Write, DisplayedBy = "Unpublished", HiddenBy = "Management", Alignment = "Right" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/FrameAction.html", RequiredPermission = Permission.Write, Alignment = "Right" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Close", Url = "{{Context.CurrentItem.PreviewUrl || Interface.Paths.PreviewUrl}}", Target = Targets.Preview, DisplayedBy = "Management", Alignment = "Right" }),
					//new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Publish", Url = "#publish", IconUrl = "redesign/img/glyphicons-white/glyphicons_063_power.png" })
				}
			};
		}

		private InterfaceTrash CreateTrash(HttpContext context)
		{
			var trash = engine.Resolve<ITrashHandler>();
			
			if (trash.TrashContainer == null)
				return new InterfaceTrash();

			var container = (ContentItem)trash.TrashContainer;

			var total = (int)engine.Persister.Repository.Count(Parameter.Below(container));
			var children = (int)engine.Persister.Repository.Count(Parameter.Equal("Parent", container));
			
			return new InterfaceTrash
			{
				Current = engine.GetContentAdapter<NodeAdapter>(container).GetTreeNode(container),
				HasChildren = children > 0,
				ChildItems = children,
				TotalItems = total,
				Children = new Node<TreeNode>[0]
			};
		}

		private InterfaceUser CreateUser(HttpContext context)
		{
			return new InterfaceUser
			{
				Name = context.User.Identity.Name,
				Username = context.User.Identity.Name,
				PreferredView = engine.Config.Sections.Management.Versions.DefaultViewMode
			};
		}

		private Node<TreeNode> CreateContent(HttpContext context)
		{
			var filter = engine.EditManager.GetEditorFilter(context.User);
			
			var structure = new BranchHierarchyBuilder(selection.SelectedItem, selection.Traverse.RootPage, true) { UseMasterVersion = false }
				.Children((item) => engine.GetContentAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing).Where(filter))
				.Build();

			return CreateStructure(structure, filter);
		}

		private Node<TreeNode> CreateStructure(HierarchyNode<ContentItem> structure, ItemFilter filter)
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

		private Node<InterfaceMenuItem> CreateMainMenu()
		{
			return new Node<InterfaceMenuItem>
			{
				Children = new Node<InterfaceMenuItem>[]
				{
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Home", Target = Targets.Preview, Url = engine.Content.Traverse.RootPage.Url, IconClass = "icon-home" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Pages", Target = "_top", Url = "{ManagementUrl}".ResolveUrlTokens() }),
					//new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Files", Target = "_top", Url = "#files" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Settings", Target = "_top", Url = "#settings" })
					{
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Site", ToolTip = "Edit site settings", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/EditRecursive.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}", RequiredPermission = Permission.Write }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Templates", ToolTip = "Show predefined templates with content", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/Templates/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}", RequiredPermission = Permission.Administer }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Wizards", ToolTip = "Show predefined types and locations for content", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/Wizard/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Users", ToolTip = "Manage users", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Users/Users.aspx", RequiredPermission = Permission.Administer }),
						}
					}
				}
			};
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}