using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;

namespace N2.Edit.Navigation
{
	public class SortDown : HelpfulHandler
	{
		public override void ProcessRequest(HttpContext context)
		{
			ContentItem selectedNode = GetSelectedItem(context.Request.QueryString);
			Engine.Resolve<ITreeSorter>().MoveDown(selectedNode);

			context.Response.Redirect("Tree.aspx?selected=" + HttpUtility.UrlEncode(selectedNode.Path));
		}
	}
}
