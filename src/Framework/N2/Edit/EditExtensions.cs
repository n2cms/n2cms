using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Definitions;
using N2.Web;
using N2.Web.Mvc.Html;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	public static class EditExtensions
	{
		/// <summary>Checks access and the drag'n'drop state before adding the creator node to the given collection.</summary>
		/// <param name="items"></param>
		/// <param name="engine"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static IEnumerable<ContentItem> TryAppendCreatorNode(this IEnumerable<ContentItem> items, IEngine engine, ContentItem parent)
		{
			var context = engine.Resolve<IWebContext>().HttpContext;
			var state = N2.Web.UI.WebControls.ControlPanel.GetState(engine.SecurityManager, context.User, context.Request.QueryString);
			if (state != ControlPanelState.DragDrop)
				return items;

			return items.AppendCreatorNode(engine, parent);
		}

		/// <summary>Appends the creator node to the given collection.</summary>
		/// <param name="items"></param>
		/// <param name="engine"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static IEnumerable<ContentItem> AppendCreatorNode(this IEnumerable<ContentItem> items, IEngine engine, ContentItem parent)
		{
			if (parent.ID == 0)
				return items;

			return items.Union(new[] { new CreatorItem(engine, parent) });
		}
	}

	internal class CreatorItem : ContentItem, ISystemNode
	{
		public CreatorItem()
		{
		}

		public CreatorItem(IEngine engine, ContentItem parent)
		{
			this.url = engine.ManagementPaths.GetSelectNewItemUrl(parent).ToUrl().AppendQuery("returnUrl", engine.Resolve<IWebContext>().HttpContext.Request.RawUrl);
			this.Title = "<span class='creator-add'>&nbsp;</span>" + (Utility.GetGlobalResourceString("Management", "Add") ?? "Add...");
		}

		string url;
		public override string Url
		{
			get { return url; }
		}
	}
}
