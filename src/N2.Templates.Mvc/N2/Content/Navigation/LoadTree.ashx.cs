using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.IO;
using N2.Web.UI.WebControls;
using N2.Web;
using N2.Collections;
using N2.Engine;
using N2.Edit.Workflow;

namespace N2.Edit.Navigation
{
	public class LoadTree : HelpfulHandler
	{
		public override void ProcessRequest(HttpContext context)
		{
            string target = context.Request["target"] ?? Targets.Preview;

			ContentItem selectedNode = GetSelectedItem(context.Request.QueryString);
			
			context.Response.ContentType = "text/plain";

			ItemFilter filter = Engine.EditManager.GetEditorFilter(context.User);
			IContentAdapterProvider adapters = Engine.Resolve<IContentAdapterProvider>();
			var root = new TreeHierarchyBuilder(selectedNode, 2)
				.Children((item) => adapters.ResolveAdapter<NodeAdapter>(item.GetType()).GetChildren(item, Interfaces.Managing))
				.Build();

			TreeNode tn = (TreeNode)new N2.Web.Tree(root)
				.LinkProvider(delegate(ContentItem node) { return N2.Edit.Web.UI.Controls.Tree.BuildLink(node, node.Path == selectedNode.Path, target); })
				.Filters(filter)
				.ToControl();
			
			Web.UI.Controls.Tree.AppendExpanderNodeRecursive(tn, filter, target);

			RenderControls(tn.Controls, context.Response.Output);
		}

		private static void RenderControls(IEnumerable controls, TextWriter output)
		{
			using (HtmlTextWriter writer = new HtmlTextWriter(output))
			{
				foreach (Control childNode in controls)
				{
					childNode.RenderControl(writer);
				}
			}
		}
	}
}
