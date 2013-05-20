using N2.Collections;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
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
		private Func<string, string> accessor;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			accessor = context.Request.GetRequestValueAccessor();
			selection = new SelectionUtility(accessor, engine);

			context.Response.ContentType = "application/json";

			switch (context.Request.HttpMethod)
			{
				case "GET":
					switch (context.Request.PathInfo)
					{
						case "/children":
						default:
							WriteChildren(context);
							break;
					}
					break;
				case "POST":
					switch (context.Request.PathInfo)
					{
						case "/sort":
						case "/move":
							Move(accessor);
							break;
					}
					break;
			}
		}

		private void Move(Func<string, string> request)
		{
			var sorter = engine.Resolve<ITreeSorter>();
			var from = selection.SelectedItem;
			if (!string.IsNullOrEmpty(request("before")))
			{
				var before = engine.Resolve<Navigator>().Navigate(request("before"));

				var ex = engine.IntegrityManager.GetMoveException(from, before.Parent);
				if (ex != null)
					throw ex;

				sorter.MoveTo(from, NodePosition.Before, before);
			}
			else
			{
				var to = engine.Resolve<Navigator>().Navigate(request("to"));
				if (to == null)
					throw new InvalidOperationException("Cannot move to null");

				var ex = engine.IntegrityManager.GetMoveException(from, to);
				if (ex != null)
					throw ex;

				sorter.MoveTo(from, to);
			}
		}

		private void WriteChildren(HttpContext context)
		{
			var children = CreateChildren(context).ToList();
			new 
			{ 
				Children = children,
				IsPaged = selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge)
			}.ToJson(context.Response.Output);
		}

		private IEnumerable<Node<TreeNode>> CreateChildren(HttpContext context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(selection.SelectedItem);
			var filter = engine.EditManager.GetEditorFilter(context.User);
			
			var query = Query.From(selection.SelectedItem);
			query.Interface = Interfaces.Managing;
			if (context.Request["pages"] != null)
				query.OnlyPages = Convert.ToBoolean(context.Request["pages"]);
			if (selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge))
				query.Limit = new Range(0, SyncChildCollectionStateAttribute.LargeCollecetionThreshold);
			if (context.Request["skip"] != null)
				query.Skip(int.Parse(context.Request["skip"]));
			if (context.Request["take"] != null)
				query.Take(int.Parse(context.Request["take"]));

			return adapter.GetChildren(query)
				.Where(filter)
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
				Expanded = false
			};
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}