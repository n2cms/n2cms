using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
	public class Content : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";

			switch (context.Request.HttpMethod)
			{
				case "GET":
					WriteChildren(context);
					break;
				case "POST":
					switch (context.Request.PathInfo)
					{
						case "/move":
							Move(context);
							break;
					}
					break;
			}
		}

		private void Move(HttpContext context)
		{
			var sorter = engine.Resolve<ITreeSorter>();
			var from = selection.SelectedItem;
			var to = engine.Resolve<Navigator>().Navigate(context.Request["to"]);
			if (to == null)
				throw new InvalidOperationException("Cannot move to null");
			if (!string.IsNullOrEmpty(context.Request["before"]))
			{
				var before = engine.Resolve<Navigator>().Navigate(context.Request["before"]);
				sorter.MoveTo(from, NodePosition.Before, before);
			}
			else
			{
				sorter.MoveTo(from, to);
			}
		}

		private void WriteChildren(HttpContext context)
		{
			var children = CreateChildren(context).ToList();
			children.ToJson(context.Response.Output);
		}

		private IEnumerable<Node<TreeNode>> CreateChildren(HttpContext context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(selection.SelectedItem);
			var filter = engine.EditManager.GetEditorFilter(context.User);
			return adapter.GetChildren(selection.SelectedItem, Interfaces.Managing)
				.Select(c => CreateNode(c, filter));
		}

		private Node<TreeNode> CreateNode(ContentItem item, ItemFilter filter)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(item);
			return new Node<TreeNode>
			{
				Current = adapter.GetTreeNode(item),
				Children = new Node<TreeNode>[0],
				HasChildren	= adapter.HasChildren(item, filter),
				Expanded = false,
				Selected = false
			};
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}