using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Engine;
using N2.Edit.Workflow;

namespace N2.Edit.Web.UI.Controls
{
	public class Tree : Control
	{
		ContentItem selectedtItem = null;
		ContentItem rootItem = null;
		ItemFilter filter = null;
		string target = Targets.Preview;
		IEngine engine;

		public Tree()
		{
		}

		public string SelectableExtensions { get; set; }

		public string SelectableTypes { get; set; }

		private static IEditUrlManager ManagementPaths
		{
			get { return N2.Context.Current.ManagementPaths; }
		}

		public HierarchyNode<ContentItem> Nodes { get; set; }

		public ContentItem SelectedItem
		{
			get { return selectedtItem ?? (selectedtItem = Find.CurrentPage ?? Find.StartPage); }
			set { selectedtItem = value; }
		}

		public ContentItem RootNode
		{
		    get { return rootItem ?? Find.RootItem; }
		    set { rootItem = value; }
		}

		public ItemFilter Filter
		{
			get { return filter ?? N2.Context.Current.EditManager.GetEditorFilter(Page.User); }
			set { filter = value; }
		}

		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		public IEngine Engine
		{
			get { return engine ?? (engine = N2.Context.Current); }
			set { engine = value; }
		}

		public override void DataBind()
		{
			EnsureChildControls();
			base.DataBind();
		}

		protected override void CreateChildControls()
		{
			IContentAdapterProvider adapters = Engine.Resolve<IContentAdapterProvider>();

			if (Nodes == null)
				Nodes = new BranchHierarchyBuilder(SelectedItem, RootNode, true)
					.Children((item) => adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing))
					.Build();

			var tree = new N2.Web.Tree(Nodes)
				.OpenTo(SelectedItem)
				.Filters(Filter)
				.LinkProvider(item => BuildLink(adapters.ResolveAdapter<NodeAdapter>(item), item, item.Path == SelectedItem.Path, Target, IsSelectable(item, SelectableTypes, SelectableExtensions)))
				.ToControl();

			AppendExpanderNodeRecursive(tree, Filter, Target, adapters, SelectableTypes, SelectableExtensions);

			Controls.Add(tree);
			
			base.CreateChildControls();
		}

		public static void AppendExpanderNodeRecursive(Control tree, ItemFilter filter, string target, IContentAdapterProvider adapters, string selectableTypes, string selectableExtensions)
		{
			TreeNode tn = tree as TreeNode;
			if (tn != null)
			{
				foreach (Control child in tn.Controls)
				{
					AppendExpanderNodeRecursive(child, filter, target, adapters, selectableTypes, selectableExtensions);
				}
				if (tn.Controls.Count == 0 && adapters.ResolveAdapter<NodeAdapter>(tn.Node).HasChildren(tn.Node, filter))
				{
					AppendExpanderNode(tn, target, selectableTypes, selectableExtensions);
				}
			}
		}

		public static void AppendExpanderNode(TreeNode tn, string target, string selectableTypes, string selectableExtensions)
		{
			Li li = new Li();
			
			li.Text = "{url:" + Url.ParseTokenized("{ManagementUrl}/Content/Navigation/LoadTree.ashx")
				.AppendQuery("target", target)
				.AppendQuery("selected", HttpUtility.UrlEncode(tn.Node.Path))
				.AppendQuery("selectableTypes", selectableTypes)
				.AppendQuery("selectableExtensions", selectableExtensions)
				+ "}";

			tn.UlClass = "ajax";
			tn.Controls.Add(li);
		}

		internal static ILinkBuilder BuildLink(NodeAdapter adapter, ContentItem item, bool isSelected, string target, bool isSelectable)
		{
			INode node = item;
			string className = node.ClassNames;
			if (isSelected)
				className += "selected ";
			if (isSelectable)
				className += "selectable ";
			else
				className += "unselectable ";

			ILinkBuilder builder = Link.To(node)
				.Target(target)
				.Class(className)
				.Href(adapter.GetPreviewUrl(item))
				.Text("<img src='" + adapter.GetIconUrl(item) + "'/>" + node.Contents)
				.Attribute("id", item.Path.Replace('/', '_'))
				.Attribute("title", "#" + item.ID + ": " + N2.Context.Current.Definitions.GetDefinition(item).Title)
				.Attribute("data-id", item.ID.ToString())
				.Attribute("data-type", item.GetContentType().Name)
				.Attribute("data-path", item.Path)
				.Attribute("data-url", item.Url)
				.Attribute("data-page", item.IsPage.ToString().ToLower())
				.Attribute("data-zone", item.ZoneName)
				.Attribute("data-permission", adapter.GetMaximumPermission(item).ToString());

			if (isSelected)
				builder.Attribute("data-selected", "true");
			if (isSelectable)
				builder.Attribute("data-selectable", "true");

			builder.Href(adapter.GetPreviewUrl(item));

			return builder;
		}

		internal static bool IsSelectable(ContentItem item, string selectableTypes, string selectableExtensions)
		{
			var baseTypesAndInterfaceNames = item.GetContentType().GetInterfaces().Select(i => i.Name)
				.Union(Utility.GetBaseTypesAndSelf(item.GetContentType()).Select(t => t.Name));

			bool isSelectableType = string.IsNullOrEmpty(selectableTypes)
				|| selectableTypes.Split(',').Intersect(baseTypesAndInterfaceNames).Any();

			bool isSelectableExtension = string.IsNullOrEmpty(selectableExtensions) 
				|| selectableExtensions.Split(',').Contains(VirtualPathUtility.GetExtension(item.Url), StringComparer.InvariantCultureIgnoreCase);
			
			return isSelectableType && isSelectableExtension;
		}
	}
}
