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

			var selection = new SelectionUtility(context.Request, N2.Context.Current);
			ContentItem selectedNode = selection.SelectedItem;
			
			context.Response.ContentType = "text/plain";

			ItemFilter filter = Engine.EditManager.GetEditorFilter(context.User);
			IContentAdapterProvider adapters = Engine.Resolve<IContentAdapterProvider>();
			var root = new TreeHierarchyBuilder(selectedNode, 2)
				.Children((item) => adapters.ResolveAdapter<NodeAdapter>(item).GetChildren(item, Interfaces.Managing))
				.Build();

			TreeNode tn = (TreeNode)new N2.Web.Tree(root)
				.LinkProvider(node => Web.UI.Controls.Tree.BuildLink(adapters.ResolveAdapter<NodeAdapter>(node), node, node.Path == selectedNode.Path, target))
				.Filters(filter)
				.ToControl();

			Web.UI.Controls.Tree.AppendExpanderNodeRecursive(tn, filter, target, adapters);

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
