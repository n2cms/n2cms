using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Web;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using System.Security.Principal;

namespace N2.Edit.Web.UI.Controls
{
	public class Tree : Control
	{
		private ContentItem selectedtItem = null;
		private ContentItem rootItem = null;
		private string target = "preview";
		private bool preview = true;

		public Tree()
		{
		}

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

		public bool Preview
		{
			get { return preview; }
			set { preview = value; }
		}

		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		public override void DataBind()
		{
			EnsureChildControls();
			base.DataBind();
		}

		protected override void CreateChildControls()
		{
			ItemFilter filter = N2.Context.Current.EditManager.GetEditorFilter(Page.User);

			Control tree = N2.Web.Tree.Between(SelectedItem, RootNode, true)
				.OpenTo(SelectedItem)
				.Filters(filter)
				.LinkProvider(BuildLink)
				.ToControl();

			AppendExpanderNodeRecursive(tree, filter);

			Controls.Add(tree);
			
			base.CreateChildControls();
		}

		public static void AppendExpanderNodeRecursive(Control tree, ItemFilter filter)
		{
			TreeNode tn = tree as TreeNode;
			if (tn != null)
			{
				foreach (Control child in tn.Controls)
				{
					AppendExpanderNodeRecursive(child, filter);
				}
				if (tn.Controls.Count == 0 && tn.Node.GetChildren(filter).Count > 0)
				{
					AppendExpanderNode(tn);
				}
			}
		}

		public static void AppendExpanderNode(TreeNode tn)
		{
			Li li = new Li();
			li.Text = "{url:../Navigation/LoadTree.ashx?selected=" + HttpUtility.UrlEncode(tn.Node.Path) + "}";

			tn.UlClass = "ajax";
			tn.Controls.Add(li);
		}

		private ILinkBuilder BuildLink(INode node)
		{
			string className = node.ClassNames;
			if (node.Path == SelectedItem.Path)
				className += "selected ";
			
			ILinkBuilder builder = Link.To(node).Target(target).Class(className)
				.Text("<img src='" + Utility.ToAbsolute(node.IconUrl) + "'/>" + node.Contents)
				.Attribute("rel", node.Path);

			if (Preview)
				builder.Href(node.PreviewUrl);

			return builder;
		}
	}
}
