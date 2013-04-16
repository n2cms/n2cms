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

	public class InterfaceContentNode
	{
		public string Type { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }

		public int ID { get; set; }

		public string Path { get; set; }
	}

	public class InterfaceData
	{
		public Node<InterfaceMenuItem> TopMenu { get; set; }
		public Node<InterfaceMenuItem> ToolbarMenu { get; set; }
		public Node<InterfaceContentNode> Content { get; set; }
	}

	/// <summary>
	/// Summary description for UI
	/// </summary>
	public class Interface : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "application/json";
			new InterfaceData
			{
				TopMenu = CreateTopMenu(),
				Content = CreateContent(context)
			}.ToJson(context.Response.Output);
			
		}

		private static Node<InterfaceContentNode> CreateContent(HttpContext context)
		{
			var selection = new SelectionUtility(context.Request, Context.Current);

			var Filter = Context.Current.EditManager.GetEditorFilter(context.User);
			var structure = new BranchHierarchyBuilder(selection.SelectedItem, N2.Find.RootItem, true) { UseMasterVersion = false }
				.Children((item) => Context.Current.Resolve<IContentAdapterProvider>().ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing).Where(Filter))
				.Build();

			return CreateStructure(structure);
		}

		private static Node<InterfaceContentNode> CreateStructure(HierarchyNode<ContentItem> structure)
		{
			return new Node<InterfaceContentNode>
			{
				Current = new InterfaceContentNode
				{
					ID = structure.Current.ID,
					Title = structure.Current.Title,
					Path = structure.Current.Path,
					Type = structure.Current.GetContentType().Name,
					Url = structure.Current.Url,
				},
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