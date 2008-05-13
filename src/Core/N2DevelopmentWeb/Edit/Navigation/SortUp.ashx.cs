using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;

namespace N2.Edit.Navigation
{
	public class SortUp : HelpfulHandler
	{

		public override void ProcessRequest(HttpContext context)
		{
			ContentItem selectedNode = GetSelectedItem(context.Request.QueryString);
			if (selectedNode.Parent != null)
			{
				IList<ContentItem> siblings = selectedNode.Parent.Children;
				int index = siblings.IndexOf(selectedNode);
				if (index > 0)
				{
					Utility.MoveToIndex(siblings, selectedNode, index - 1);
					foreach (ContentItem changed in Utility.UpdateSortOrder(siblings))
						Engine.Persister.Save(changed);
				}
			}
			context.Response.Redirect("Tree.aspx?selected=" + HttpUtility.UrlEncode(selectedNode.Path));
		}
	}
}
