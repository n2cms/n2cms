using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Web;
using N2.Web.UI;

namespace N2.Edit.Web.UI.Controls
{
	public class Tree : Control
	{
		private ContentItem currentItem = null;
		private ContentItem rootItem = null;
		private string target = "preview";

		public Tree()
		{
		}

		public ContentItem CurrentItem
		{
			get { return currentItem ?? Find.CurrentPage; }
			set { currentItem = value; }
		}

		public ContentItem RootNode
		{
			get { return rootItem ?? Find.RootItem; }
			set { rootItem = value; }
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
			Control ul = N2.Web.Tree.From(RootNode)
				.OpenTo(CurrentItem ?? Find.StartPage)
				.Filters(GetFilters())
				.LinkProvider(BuildLink)
				.ToControl();
			Controls.Add(ul);

			base.CreateChildControls();
		}

		private ItemFilter[] GetFilters()
		{
			bool displayDataItems = N2.Context.Current.Resolve<Settings.NavigationSettings>().DisplayDataItems;
			return displayDataItems
					? new ItemFilter[] { new AccessFilter(Page.User, N2.Context.SecurityManager) }
					: new ItemFilter[] { new PageFilter(), new AccessFilter(Page.User, N2.Context.SecurityManager) };
		}

		private ILinkBuilder BuildLink(INode node)
		{
			string className = node.ClassNames;
			if (node.Path == CurrentItem.Path)
				className += "selected ";
			
			ILinkBuilder builder = Link.To(node).Target(target).Class(className)
				.Text("<img src='" + Utility.ToAbsolute(node.IconUrl) + "'/>" + node.Contents)
				.Attribute("rel", node.Path);

			return builder;
		}
	}
}
