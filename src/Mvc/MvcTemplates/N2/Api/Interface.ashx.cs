using N2.Collections;
using N2.Edit;
using N2.Engine;
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
	}

	public class InterfaceMenuItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public string Frame { get; set; }

		public string IconClass { get; set; }
	}

	public class InterfaceData
	{
		public Node<InterfaceMenuItem> TopMenu { get; set; }
		public Node<InterfaceMenuItem> ToolbarMenu { get; set; }
		public Node<TreeNode> Content { get; set; }

		public Site Site { get; set; }

		public string Authority { get; set; }

		public InterfaceUser User { get; set; }
	}

	public class InterfaceUser
	{
		public string Name { get; set; }
		public string Username { get; set; }
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
				Content = CreateContent(context),
				Site = engine.Host.GetSite(selection.SelectedItem),
				Authority = context.Request.Url.Authority,
				User = CreateUser(context)
			}.ToJson(context.Response.Output);
			
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

			return CreateStructure(structure);
		}

		private Node<TreeNode> CreateStructure(HierarchyNode<ContentItem> structure)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(structure.Current);
			return new Node<TreeNode>
			{
				Current = adapter.GetTreeNode(structure.Current),
				Children = structure.Children.Select(c => CreateStructure(c)).ToArray()
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