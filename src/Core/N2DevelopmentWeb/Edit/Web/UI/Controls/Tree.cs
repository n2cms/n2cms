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
	public class Tree : Control, IItemContainer
	{
		private ContentItem currentItem = null;
		private ContentItem rootItem = null;
		private string target = "preview";
		private readonly IList<string> icons = new List<string>();

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
			ItemFilter[] filters = GetFilters();
			N2.Web.Tree t = N2.Web.Tree.From(RootNode)
				.OpenTo(CurrentItem ?? Find.StartPage).Filters(filters)
				.LinkProvider(BuildLink);
			Control ul = t.ToControl();
			Controls.Add(ul);

			for (int i = 0; i < icons.Count; i++)
			{
				Page.ClientScript.RegisterArrayDeclaration("icons", string.Format("'{0}'", VirtualPathUtility.ToAbsolute(icons[i])));
			}

			base.CreateChildControls();
		}

		private ItemFilter[] GetFilters()
		{
			bool displayDataItems = N2.Context.Instance.Resolve<Settings.NavigationSettings>().DisplayDataItems;
			return displayDataItems
					? new ItemFilter[] { new AccessFilter(Page.User, N2.Context.SecurityManager) }
					: new ItemFilter[] { new PageFilter(), new AccessFilter(Page.User, N2.Context.SecurityManager) };
		}

		private ILinkBuilder BuildLink(ContentItem item)
		{
			StringBuilder className = new StringBuilder();

			if (!item.Published.HasValue || item.Published > DateTime.Now)
				className.Append("unpublished ");
			else if (item.Published > DateTime.Now.AddDays(-1))
				className.Append("day ");
			else if (item.Published > DateTime.Now.AddDays(-7))
				className.Append("week ");
			else if (item.Published > DateTime.Now.AddMonths(-1))
				className.Append("month ");

			if (item.Expires.HasValue && item.Expires <= DateTime.Now)
				className.Append("expired ");

			if (item == CurrentItem)
				className.Append("selected ");

			if (!item.Visible)
				className.Append("invisible ");

			if (item.AuthorizedRoles != null && item.AuthorizedRoles.Count > 0)
				className.Append("locked ");

			string iconUrl = item.IconUrl;
			int iconIndex = icons.IndexOf(iconUrl);
			if (iconIndex < 0)
			{
				iconIndex = icons.Count;
				icons.Add(iconUrl);
			}
			className.Append("i" + iconIndex);

			ILinkBuilder builder = Link.To(item).Target(target).Href(item.RewrittenUrl).Class(className.ToString()).Attribute("rel", item.Path);

			return builder;
		}
	}
}
