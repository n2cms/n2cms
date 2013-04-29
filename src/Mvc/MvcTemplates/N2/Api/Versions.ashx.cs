using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
	public class Versions : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";

			var versions = CreateVersions(context).ToList();
			versions.ToJson(context.Response.Output);
		}

		private IEnumerable<Node<TreeNode>> CreateVersions(HttpContext context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(selection.SelectedItem);
			var versions = engine.Resolve<IVersionManager>().GetVersionsOf(selection.SelectedItem);
			return versions.Select(v => new Node<TreeNode>(adapter.GetTreeNode(v)));
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}