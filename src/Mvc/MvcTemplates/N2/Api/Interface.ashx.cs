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
	}

	//public class InterfaceContentNode
	//{
	//	public string Type { get; set; }
	//	public string Title { get; set; }
	//	public string Url { get; set; }

	//	public int ID { get; set; }

	//	public string Path { get; set; }

	//	public string IconUrl { get; set; }

	//	public string Target { get; set; }
	//}

	public class InterfaceData
	{
		public Node<InterfaceMenuItem> TopMenu { get; set; }
		public Node<InterfaceMenuItem> ToolbarMenu { get; set; }
		public Node<TreeNode> Content { get; set; }

		public Site Site { get; set; }

		public string Authority { get; set; }
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
				Authority = context.Request.Url.Authority
			}.ToJson(context.Response.Output);
			
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
			var node = adapter.GetTreeNode(structure.Current);
			return new Node<TreeNode>
			{
				//Current = new InterfaceContentNode
				//{
				//	ID = structure.Current.ID,
				//	Title = structure.Current.Title,
				//	Path = structure.Current.Path,
				//	Type = structure.Current.GetContentType().Name,
				//	Url = structure.Current.Url,
				//	IconUrl = structure.Current.IconUrl,
				//	Target = "preview",

				//},
				Current = node,
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
							Current = new InterfaceMenuItem { Title = "Dashboard", Frame = "_top", Url = "#dashboard" },
							Children = new Node<InterfaceMenuItem>[0]
						},
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "Content", Frame = "_top", Url = "#content" },
							Children = new Node<InterfaceMenuItem>[]
							{
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Pages", Frame = "_top", Url = "#content/pages" },
									Children = new Node<InterfaceMenuItem>[0]
								},
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Files", Frame = "_top", Url = "#content/files" },
									Children = new Node<InterfaceMenuItem>[0]
								},
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Wizards", Frame = "_top", Url = "#content/wizards" },
									Children = new Node<InterfaceMenuItem>[0]
								},
								new Node<InterfaceMenuItem>
								{
									Current = new InterfaceMenuItem { Title = "Templates", Frame = "_top", Url = "#content/templates" },
									Children = new Node<InterfaceMenuItem>[0]
								},
							}
						},
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "Site", Frame = "_top", Url = "#site" },
							Children = new Node<InterfaceMenuItem>[0]
						},
						new Node<InterfaceMenuItem>
						{
							Current = new InterfaceMenuItem { Title = "System", Frame = "_top", Url = "#system" },
							Children = new Node<InterfaceMenuItem>[0]
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