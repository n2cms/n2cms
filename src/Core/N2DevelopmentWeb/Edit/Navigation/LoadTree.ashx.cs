using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.UI;
using N2.Web.UI.WebControls;
using N2.Web;

namespace N2.Edit.Navigation
{
	public class LoadTree : IHttpHandler
	{
		ContentItem selectedNode;

		public void ProcessRequest(HttpContext context)
		{
			string path = context.Request["selected"];
			selectedNode = N2.Context.Current.Resolve<N2.Edit.Navigator>().Navigate(path);
			
			context.Response.ContentType = "text/plain";
			TreeNode tn = (TreeNode)N2.Web.Tree
				.From(selectedNode, 2)
				.LinkProvider(BuildLink)
				.ToControl();
			tn.ChildrenOnly = true;
			Web.UI.Controls.Tree.AppendExpanderNodeRecursive(tn, Web.UI.Controls.Tree.GetFilters(context.User));
			tn.RenderControl(new HtmlTextWriter(context.Response.Output));
		}

		public ILinkBuilder BuildLink(INode node)
		{
			string className = node.ClassNames;

			ILinkBuilder builder = Link.To(node).Target("preview").Class(className)
				.Text("<img src='" + Utility.ToAbsolute(node.IconUrl) + "'/>" + node.Contents)
				.Attribute("rel", node.Path);

			return builder;
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
