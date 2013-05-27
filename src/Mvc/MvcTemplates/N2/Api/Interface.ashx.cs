using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
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

		public string SelectedPath { get; set; }

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
				SelectedPath = selection.SelectedItem.Path,
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
				ViewPreference = new HttpContextWrapper(context).GetViewPreference(engine.Config.Sections.Management.Versions.DefaultViewMode).ToString()
			};
		}

		private Node<InterfaceMenuItem> CreateActionMenu(HttpContext context)
		{
			return new Node<InterfaceMenuItem>
			{
				Children = new Node<InterfaceMenuItem>[]
				{
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Url = "{{Context.CurrentItem.PreviewUrl}}", IconUrl = "redesign/img/glyphicons-white/glyphicons_051_eye_open.png" })
					{
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "View latest drafts", Target = "", Url = "{{Interface.Paths.Management}}?view=draft&{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "View published versions", Target = "", Url = "{{Interface.Paths.Management}}?view=published&{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Show links", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/LinkTracker/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
						}
					},
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Edit", Description = "Page details", Url = "{{Interface.Paths.Edit}}?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}", IconUrl = "redesign/img/glyphicons-white/glyphicons_150_edit.png" })
					{
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Edit {{Context.CurrentItem.Title}}", Url = "{{Interface.Paths.Edit}}?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}", IconUrl = "redesign/img/glyphicons-black/glyphicons_150_edit.png" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Organize parts on {{Context.CurrentItem.Title}}", Url = "{{Context.CurrentItem.PreviewUrl}}&edit=drag", IconUrl = "redesign/img/glyphicons-black/glyphicons_154_more_windows.png" }),
						}
					},
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/PageVersions.html" }),
					new Node<InterfaceMenuItem>(new InterfaceMenuItem { TemplateUrl = "App/Partials/PageLanguage.html" }),
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
				.Children((item) => engine.Resolve<IContentAdapterProvider>().ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing).Where(filter))
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
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Site", ToolTip = "Edit site settings", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/EditRecursive.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Templates", ToolTip = "Show predefined templates with content", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/Templates/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Wizards", ToolTip = "Show predefined types and locations for content", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Content/Wizard/Default.aspx?{{Interface.Paths.SelectedQueryKey}}={{Context.CurrentItem.Path}}&id={{Context.CurrentItem.ID}}" }),
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = "Users", ToolTip = "Manage users", Target = Targets.Preview, Url = "{{Interface.Paths.Management}}/Users/Users.aspx" }),
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