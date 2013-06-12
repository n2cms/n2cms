using N2.Collections;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

			if (!engine.SecurityManager.IsAuthorized(context.User, selection.SelectedItem, Security.Permission.Read))
				throw new UnauthorizedAccessException();

			switch (context.Request.HttpMethod)
			{
				case "GET":
					switch (context.Request.PathInfo)
					{
						case "/search":
							WriteSearch(context);
							break;
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
							Move(context, accessor);
							break;
						case "/copy":
							Copy(context, accessor);
							break;
						case "/delete":
							Delete(context);
							break;
						case "/publish":
							Publish(context);
							break;
						case "/unpublish":
							Unpublish(context);
							break;
						case "/schedule":
							Schedule(context);
							break;
					}
					break;
				case "DELETE":
					Delete(context);
					break;
			}
		}

		private void Schedule(HttpContext context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, selection.SelectedItem, Security.Permission.Publish))
				throw new UnauthorizedAccessException();

			var publishDate = DateTime.Parse(context.Request["publishDate"]);
			selection.SelectedItem.SchedulePublishing(publishDate, engine);

			context.Response.WriteJson(new { Scheduled = true, Current = engine.GetNodeAdapter(selection.SelectedItem).GetTreeNode(selection.SelectedItem) });
		}

		private void Publish(HttpContext context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, selection.SelectedItem, Security.Permission.Publish))
				throw new UnauthorizedAccessException();

			var item = engine.Resolve<IVersionManager>().Publish(engine.Persister, selection.SelectedItem);

			context.Response.WriteJson(new { Published = true, Current = engine.GetNodeAdapter(selection.SelectedItem).GetTreeNode(selection.SelectedItem) });
		}

		private void Unpublish(HttpContext context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, selection.SelectedItem, Security.Permission.Publish))
				throw new UnauthorizedAccessException();

			selection.SelectedItem.Expires = DateTime.Now;
			engine.Resolve<StateChanger>().ChangeTo(selection.SelectedItem, ContentState.Unpublished);
			engine.Persister.Save(selection.SelectedItem);

			context.Response.WriteJson(new { Unpublished = true, Current = engine.GetNodeAdapter(selection.SelectedItem).GetTreeNode(selection.SelectedItem) });
		}

		private void WriteSearch(HttpContext context)
		{
			var q = N2.Persistence.Search.Query.Parse(new HttpRequestWrapper(context.Request));
			var result = engine.Content.Search.Text.Search(q);

			context.Response.WriteJson(new
			{
				Total = result.Total,
				Hits = result
					.Where(i => engine.SecurityManager.IsAuthorized(i, context.User))
					.Select(i => engine.GetContentAdapter<NodeAdapter>(i).GetTreeNode(i))
					.ToList()
			});
		}

		private void Delete(HttpContext context)
		{
			var item = selection.ParseSelectionFromRequest();
			var ex = engine.IntegrityManager.GetDeleteException(item);
			if (ex != null)
				throw ex;

			if (!engine.SecurityManager.IsAuthorized(context.User, item, item.IsPublished() ? Security.Permission.Publish : Security.Permission.Write))
				throw new UnauthorizedAccessException();

			engine.Persister.Delete(item);

			var deleted = engine.Persister.Get(item.ID);
			
			if (deleted != null)
				context.Response.WriteJson(new { RemovedPermanently = false, Current = engine.GetContentAdapter<NodeAdapter>(deleted).GetTreeNode(deleted) });
			else
				context.Response.WriteJson(new { RemovedPermanently = true });
		}

		private void Move(HttpContext context, Func<string, string> request)
		{
			var sorter = engine.Resolve<ITreeSorter>();
			var from = selection.ParseSelectionFromRequest();
			if (!string.IsNullOrEmpty(request("before")))
			{
				var before = engine.Resolve<Navigator>().Navigate(request("before"));
				
				PerformMoveChecks(context, from, before.Parent);

				sorter.MoveTo(from, NodePosition.Before, before);
			}
			else
			{
				var to = engine.Resolve<Navigator>().Navigate(request("to"));

				PerformMoveChecks(context, from, to);

				sorter.MoveTo(from, to);
			}

			context.Response.WriteJson(new { Moved = true, Current = engine.GetNodeAdapter(from).GetTreeNode(from) });
		}

		private void PerformMoveChecks(HttpContext context, ContentItem from, ContentItem to)
		{
			if (to == null)
				throw new InvalidOperationException("Cannot move to null");

			var ex = engine.IntegrityManager.GetMoveException(from, to);
			if (ex != null)
				throw ex;

			if (!engine.SecurityManager.IsAuthorized(context.User, to, Security.Permission.Publish))
				throw new UnauthorizedAccessException();
		}

		private void Copy(HttpContext context, Func<string, string> request)
		{
			ContentItem newItem;
			var from = selection.ParseSelectionFromRequest();
			if (!string.IsNullOrEmpty(request("before")))
			{
				var before = engine.Resolve<Navigator>().Navigate(request("before"));
				
				newItem = PerformCopy(context, from, before.Parent);

				var sorter = engine.Resolve<ITreeSorter>();
				sorter.MoveTo(newItem, NodePosition.Before, before);
			}
			else
			{
				var to = engine.Resolve<Navigator>().Navigate(request("to"));
				newItem = PerformCopy(context, from, to);
			}

			context.Response.WriteJson(new { Copied = true, Current = engine.GetNodeAdapter(newItem).GetTreeNode(newItem) });
		}

		private ContentItem PerformCopy(HttpContext context, ContentItem from, ContentItem to)
		{
			if (to == null)
				throw new InvalidOperationException("Cannot move to null");

			ContentItem newItem;
			var ex = engine.IntegrityManager.GetCopyException(from, to);
			if (ex != null)
				throw ex;

			if (!engine.SecurityManager.IsAuthorized(context.User, to, Security.Permission.Write))
				throw new UnauthorizedAccessException();

			newItem = engine.Persister.Copy(selection.SelectedItem, to);

			if (engine.SecurityManager.IsAuthorized(context.User, to, Security.Permission.Publish))
				return newItem;

			engine.Resolve<StateChanger>().ChangeTo(newItem, ContentState.Draft);
			engine.Persister.Save(newItem);

			return newItem;
		}

		private void WriteChildren(HttpContext context)
		{
			var children = CreateChildren(context).ToList();
			context.Response.WriteJson(new  { Children = children, IsPaged = selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge) });
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