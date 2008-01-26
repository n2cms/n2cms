using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Collections;
using N2.Persistence.NH.Finder;

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
		private ClassProviderDelegate classProvider;
		private ItemFilter[] filters = null;

		#region Constructor

		public Tree(HierarchyBuilder builder)
		{
			this.builder = builder;
			linkProvider = Link.To;
			classProvider = delegate
			                	{
			                		return string.Empty;
			                	};
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

		#endregion

		#region Static Methods

		public static Tree From(ContentItem root)
		{
			Tree t = new Tree(new TreeHierarchyBuilder(root));
			return t;
		}

		public static Tree Between(ContentItem initialItem, ContentItem lastAncestor)
		{
			Tree t = new Tree(new BranchHierarchyBuilder(initialItem, lastAncestor));
			return t;
		}

		#endregion

		#region ToString
		public override string ToString()
		{
			IHierarchyNavigator<ContentItem> navigator = new ItemHierarchyNavigator(builder, filters);

			StringBuilder sb = new StringBuilder();

			using (new StringWrapper(sb, "<ul>", "</ul>"))
			{
				AppendRecursive(navigator, sb);
			}

			return sb.ToString();
		}

		private void AppendRecursive(IHierarchyNavigator<ContentItem> position, StringBuilder sb)
		{
			string liStartTag = GetLiStartTag(position);

			using (new StringWrapper(sb, liStartTag, "</li>"))
			{
				string link = linkProvider(position.Current).ToString();
				sb.Append(link);

				if (position.HasChildren)
				{
					using (new StringWrapper(sb, "<ul>", "</ul>"))
					{
						foreach (IHierarchyNavigator<ContentItem> childNavigator in position.Children)
						{
							AppendRecursive(childNavigator, sb);
						}
					}
				}
			}
		}

		private string GetLiStartTag(IHierarchyNavigator<ContentItem> position)
		{
			string className = classProvider(position.Current);
			return string.IsNullOrEmpty(className) 
			       	? "<li>" 
			       	: string.Format("<li class=\"{0}\">", className);
		}

		#endregion

		#region ToControl

		public Control ToControl()
		{
			IHierarchyNavigator<ContentItem> navigator = new ItemHierarchyNavigator(builder, filters);

			Control ul = new HtmlGenericControl("ul");
			AddRecursive(navigator, ul);
			return ul;
		}

		private void AddRecursive(IHierarchyNavigator<ContentItem> position, Control container)
		{
			HtmlGenericControl li = new HtmlGenericControl("li");
			container.Controls.Add(li);
			
			string className = classProvider(position.Current);
			if(!string.IsNullOrEmpty(className) )
				li.Attributes["class"] = className;

			li.Controls.Add(linkProvider(position.Current).ToControl());

			if(position.HasChildren)
			{
				Control ul = new HtmlGenericControl("ul");
				li.Controls.Add(ul);
				foreach (IHierarchyNavigator<ContentItem> childNavigator in position.Children)
				{
					AddRecursive(childNavigator, ul);
				}
			}
		}

		#endregion
	}
}
