using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Collections;
using N2.Persistence.NH.Finder;
using N2.Web.UI.WebControls;
using System.IO;

namespace N2.Web
{
	/// <summary>
	/// Creates a hierarchical tree of ul and li:s for usage on web pages.
	/// </summary>
	public class Tree
	{
		private readonly HierarchyBuilder builder;

		public delegate ILinkBuilder LinkProviderDelegate(ContentItem currentItem);
		public delegate string ClassProviderDelegate(ContentItem currentItem);

		private LinkProviderDelegate linkProvider;
		private ClassProviderDelegate classProvider = delegate { return string.Empty; };
		private ItemFilter[] filters = null;
		private bool exclude = false;

		#region Constructor

		public Tree(HierarchyBuilder builder)
		{
			this.builder = builder;
			linkProvider = Link.To;
		}

		#endregion

		#region Methods

		public Tree LinkProvider(LinkProviderDelegate linkProvider)
		{
			this.linkProvider = linkProvider;
			return this;
		}

		public Tree ClassProvider(ClassProviderDelegate classProvider)
		{
			this.classProvider = classProvider;
			return this;
		}

		public Tree OpenTo(ContentItem item)
		{
			IList<ContentItem> items = Find.ListParents(item);
			return ClassProvider(delegate(ContentItem current)
			                	{
			                		return items.Contains(current) || current == item
			                		       	? "open"
			                		       	: string.Empty;
			                	});
		}

		public Tree Filters(params ItemFilter[] filters)
		{
			this.filters = filters;
			return this;
		}

		public Tree ExcludeRoot(bool exclude)
		{
			this.exclude = exclude;
			return this;
		}

		#endregion

		#region Static Methods

		public static Tree From(ContentItem root)
		{
			Tree t = new Tree(new TreeHierarchyBuilder(root));
			return t;
		}

		public static Tree From(ContentItem root, int depth)
		{
			Tree t = new Tree(new TreeHierarchyBuilder(root, depth));
			return t;
		}

		public static Tree Between(ContentItem initialItem, ContentItem lastAncestor)
		{
			return new Tree(new BranchHierarchyBuilder(initialItem, lastAncestor));
		}

		public static Tree Between(ContentItem initialItem, ContentItem lastAncestor, bool appendAdditionalLevel, int startingDepth)
		{
			lastAncestor = Find.AtLevel(initialItem, lastAncestor, startingDepth);
			return new Tree(new BranchHierarchyBuilder(initialItem, lastAncestor ?? initialItem, lastAncestor != null && appendAdditionalLevel));
		}

		public static Tree Between(ContentItem initialItem, ContentItem lastAncestor, bool appendAdditionalLevel)
		{
			return new Tree(new BranchHierarchyBuilder(initialItem, lastAncestor, appendAdditionalLevel));
		}

		#endregion

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb)))
			{
				Control root = ToControl();
				root.RenderControl(writer);
			}
			return sb.ToString();
		}

		public Control ToControl()
		{
			TreeNode rootNode = BuildNodesRecursive(builder.Build(filters));
			rootNode.ChildrenOnly = exclude;
			return rootNode;
		}

		private TreeNode BuildNodesRecursive(HierarchyNode<ContentItem> navigator)
		{
			ContentItem item = navigator.Current;

			TreeNode node = new TreeNode(item, linkProvider(item).ToControl());
			node.LiClass = classProvider(item);

			foreach (var childNavigator in navigator.Children)
			{
				TreeNode childNode = BuildNodesRecursive(childNavigator);
				node.Controls.Add(childNode);
			}
			return node;
		}
	}
}
