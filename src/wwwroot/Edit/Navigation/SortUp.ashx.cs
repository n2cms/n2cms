using System;
using System.Web;

namespace N2.Edit.Navigation
{
	[SortPlugin]
	public class SortUp : HelpfulHandler
	{
		public override void ProcessRequest(HttpContext context)
		{
			ContentItem selectedNode = GetSelectedItem(context.Request.QueryString);
			Engine.Resolve<ITreeSorter>().MoveUp(selectedNode);

			context.Response.Redirect("Tree.aspx?selected=" + HttpUtility.UrlEncode(selectedNode.Path));
		}
	}
}
