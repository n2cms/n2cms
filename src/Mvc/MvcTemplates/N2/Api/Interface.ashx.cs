using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
	public class Node<T>
	{
		public T Current { get; set; }
		public IEnumerable<Node<T>> Children { get; set; }

		public bool HasChildren { get; set; }

		public bool Expanded { get; set; }

		public bool Selected { get; set; }
	}

	public class InterfaceMenuItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public string Frame { get; set; }

		public string IconClass { get; set; }

		public string Description { get; set; }

		public string ToolTip { get; set; }

		public string IconUrl { get; set; }
	}

	public class InterfaceData
	{
		public Node<InterfaceMenuItem> TopMenu { get; set; }
		public Node<InterfaceMenuItem> ToolbarMenu { get; set; }
		public Node<TreeNode> Content { get; set; }

		public Site Site { get; set; }

		public string Authority { get; set; }

		public InterfaceUser User { get; set; }

		public InterfaceTrash Trash { get; set; }

		public Node<InterfaceMenuItem> ActionMenu { get; set; }
	}

	public class InterfaceUser
	{
		public string Name { get; set; }
		public string Username { get; set; }
	}

	public class InterfaceTrash : Node<TreeNode>
	{
		public int TotalItems { get; set; }
		public int ChildItems { get; set; }
	}

	/// <summary>
	/// Summary description for UI
	/// </summary>
	public class Interface : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";
			new InterfaceData
			{
				TopMenu = CreateTopMenu(),
				ActionMenu = CreateActionMenu(context),
				Content = CreateContent(context),
				Site = engine.Host.GetSite(selection.SelectedItem),
				Authority = context.Request.Url.Authority,
				User = CreateUser(context),
				Trash = CreateTrash(context)
			}.ToJson(context.Response.Output);
		}

		private Node<InterfaceMenuItem> CreateActionMenu(HttpContext context)
		{
			return new Node<InterfaceMenuItem>
			{
				Children = new Node<InterfaceMenuItem>[]
				{
					new Node<InterfaceMenuItem>
					{
						Current = new InterfaceMenuItem { Title = "", ToolTip = "View page preview", Frame = "preview", Url = "#view", IconUrl = "redesign/img/glyphicons-white/glyphicons_051_eye_open.png" },
						Children = new Node<InterfaceMenuItem>[0]
					},
					new Node<InterfaceMenuItem>
					{
						Current = new InterfaceMenuItem { Title = "Edit", Description = "Page details", Frame = "preview", Url = "#edit", IconUrl = "redesign/img/glyphicons-white/glyphicons_150_edit.png" },
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>
							{
								Current = new InterfaceMenuItem { Title = "Organize Parts", Frame = "preview", Url = "#edit/parts", IconUrl = "redesign/img/glyphicons-white/glyphicons_154_more_windows.png" },
								Children = new Node<InterfaceMenuItem>[0]
							}
						}
					},
					new Node<InterfaceMenuItem>
					{
						Current = new InterfaceMenuItem { Title = "Versions", Description = "Published version", Frame = "preview", Url = "#versions", IconUrl = "redesign/img/glyphicons-white/glyphicons_057_history.png" },
						Children = new Node<InterfaceMenuItem>[0]
					},
					new Node<InterfaceMenuItem>
					{
						Current = new InterfaceMenuItem { Title = "Language", Description = "English", Frame = "preview", Url = "#languages", IconUrl = "redesign/img/glyphicons-white/glyphicons_370_globe_af.png" },
						Children = new Node<InterfaceMenuItem>[]
						{
							new Node<InterfaceMenuItem>
							{
								Current = new InterfaceMenuItem { Title = "English", Frame = "preview", Url = "#languages/english" },
								Children = new Node<InterfaceMenuItem>[0]
							},
							new Node<InterfaceMenuItem>
							{
								Current = new InterfaceMenuItem { Title = "Swedish", Frame = "preview", Url = "#languages/swedish" },
								Children = new Node<InterfaceMenuItem>[0]
							}
						}
					},
					new Node<InterfaceMenuItem>
					{
						Current = new InterfaceMenuItem { Title = "Publish", Frame = "preview", Url = "#publish", IconUrl = "redesign/img/glyphicons-white/glyphicons_063_power.png" },
						Children = new Node<InterfaceMenuItem>[0]
					}
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
				Username = context.User.Identity.Name
			};
		}

		private Node<TreeNode> CreateContent(HttpContext context)
		{
			var filter = engine.EditManager.GetEditorFilter(context.User);
			
			var structure = new BranchHierarchyBuilder(selection.SelectedItem, selection.Traverse.StartPage, true) { UseMasterVersion = false }
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
				Selected = selection.SelectedItem == structure.Current,
				Children = children
			};
		}

		private static Node<InterfaceMenuItem> CreateTopMenu()
		{
			return new Node<InterfaceMenuItem>
			{
				Children = new Node<InterfaceMenuItem>[]
					{
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "Home", Frame = "_top", Url = "#home", IconClass = "icon-home" },
							Children = new Node<InterfaceMenuItem>[0]
						},
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "Pages", Frame = "_top", Url = "#pages" },
							Children = new Node<InterfaceMenuItem>[0]
						},
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "Files", Frame = "_top", Url = "#files" },
							Children = new Node<InterfaceMenuItem>[0]
						},
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "Settings", Frame = "_top", Url = "#settings" },
							Children = new Node<InterfaceMenuItem>[]
							{
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Site", Frame = "_top", Url = "#settings/site" },
									Children = new Node<InterfaceMenuItem>[0]
								},
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Templates", Frame = "_top", Url = "#settings/templates" },
									Children = new Node<InterfaceMenuItem>[0]
								},
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Wizards", Frame = "_top", Url = "#settings/wizards" },
									Children = new Node<InterfaceMenuItem>[0]
								}
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