using N2.Collections;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Persistence.Sources;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace N2.Management.Api
{
	[Service(typeof(IApiHandler))]
	public class ContentHandler : IHttpHandler, IApiHandler
	{
		public ContentHandler()
			: this(Context.Current)
		{
		}

		public ContentHandler(IEngine engine)
		{
			this.engine = engine;
		}

		private IEngine engine;
		private SelectionUtility Selection { get { return engine.RequestContext.HttpContext.GetSelectionUtility(engine); } }

		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(context.GetHttpContextBase());
		}

		public void ProcessRequest(HttpContextBase context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, Selection.SelectedItem, Security.Permission.Read))
				throw new UnauthorizedAccessException();

			switch (context.Request.HttpMethod)
			{
				case "GET":
					switch (context.Request.PathInfo)
					{
						case "/search":
							WriteSearch(context);
							break;
						case "/translations":
							var translations = CreateTranslations(context).ToList();
							context.Response.WriteJson(new { Translations = translations });
							break;
						case "/versions":
							var versions = CreateVersions(context).ToList();
							context.Response.WriteJson(new { Versions = versions });
							break;
						case "/definitions":
							var definitions = CreateDefinitions(context)
								.Select(d => new { d.Title, d.Description, d.Discriminator, d.ToolTip, d.IconUrl, d.IconClass, TypeName = d.ItemType.Name })
								.ToList();
							context.Response.WriteJson(new { Definitions = definitions });
							break;
						case "/children":
						default:
							var children = CreateChildren(context).ToList();
							context.Response.WriteJson(new { Children = children, IsPaged = Selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge) });
							break;
					}
					break;
				case "POST":
					switch (context.Request.PathInfo)
					{
						case "/sort":
						case "/move":
							Move(context, Selection.RequestValueAccessor);
							break;
						case "/copy":
							Copy(context, Selection.RequestValueAccessor);
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

		private IEnumerable<ItemDefinition> CreateDefinitions(HttpContextBase context)
		{
			var item = Selection.ParseSelectionFromRequest();
			if (item != null)
				return engine.Definitions.GetAllowedChildren(item, null).WhereAuthorized(engine.SecurityManager, context.User, item);
			else
				return engine.Definitions.GetDefinitions();
		}

		private IEnumerable<Node<TreeNode>> CreateVersions(HttpContextBase context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(Selection.SelectedItem);
			var versions = engine.Resolve<IVersionManager>().GetVersionsOf(Selection.SelectedItem);
			return versions.Select(v => new Node<TreeNode>(adapter.GetTreeNode(v, allowDraft: false)));
		}

		private IEnumerable<Node<InterfaceMenuItem>> CreateTranslations(HttpContextBase context)
		{
			var languages = engine.Resolve<ILanguageGateway>();
			return languages.GetEditTranslations(Selection.SelectedItem, true, true)
				.Select(t => new Node<InterfaceMenuItem>(new InterfaceMenuItem
				{
					Title = t.Language.LanguageTitle,
					Url = t.EditUrl,
					IconClass = "sprite " + (t.Language.LanguageCode.Split('-').LastOrDefault() ?? string.Empty).ToLower()
				}));
		}

		private void Schedule(HttpContextBase context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, Selection.SelectedItem, Security.Permission.Publish))
				throw new UnauthorizedAccessException();

			if (string.IsNullOrEmpty(Selection.RequestValueAccessor("publishDate")))
			{
				context.Response.WriteJson(new { Scheduled = false });
				return;
			}
			else
			{
				var publishDate = DateTime.Parse(Selection.RequestValueAccessor("publishDate"));
				Selection.SelectedItem.SchedulePublishing(publishDate, engine);

				context.Response.WriteJson(new { Scheduled = true, Current = engine.GetNodeAdapter(Selection.SelectedItem).GetTreeNode(Selection.SelectedItem) });
			}
		}

		private void Publish(HttpContextBase context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, Selection.SelectedItem, Security.Permission.Publish))
				throw new UnauthorizedAccessException();

			var item = engine.Resolve<IVersionManager>().Publish(engine.Persister, Selection.SelectedItem);

			context.Response.WriteJson(new { Published = true, Current = engine.GetNodeAdapter(Selection.SelectedItem).GetTreeNode(Selection.SelectedItem) });
		}

		private void Unpublish(HttpContextBase context)
		{
			if (!engine.SecurityManager.IsAuthorized(context.User, Selection.SelectedItem, Security.Permission.Publish))
				throw new UnauthorizedAccessException();

			Selection.SelectedItem.Expires = DateTime.Now;
			engine.Resolve<StateChanger>().ChangeTo(Selection.SelectedItem, ContentState.Unpublished);
			engine.Persister.Save(Selection.SelectedItem);

			context.Response.WriteJson(new { Unpublished = true, Current = engine.GetNodeAdapter(Selection.SelectedItem).GetTreeNode(Selection.SelectedItem) });
		}

		private void WriteSearch(HttpContextBase context)
		{
			var q = N2.Persistence.Search.Query.Parse(context.Request);
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

		private void Delete(HttpContextBase context)
		{
			var item = Selection.ParseSelectionFromRequest();
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

		private void Move(HttpContextBase context, Func<string, string> request)
		{
			var sorter = engine.Resolve<ITreeSorter>();
			var from = Selection.ParseSelectionFromRequest();
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

		private void PerformMoveChecks(HttpContextBase context, ContentItem from, ContentItem to)
		{
			if (to == null)
				throw new InvalidOperationException("Cannot move to null");

			var ex = engine.IntegrityManager.GetMoveException(from, to);
			if (ex != null)
				throw ex;

			if (!engine.SecurityManager.IsAuthorized(context.User, to, Security.Permission.Publish))
				throw new UnauthorizedAccessException();
		}

		private void Copy(HttpContextBase context, Func<string, string> request)
		{
			ContentItem newItem;
			var from = Selection.ParseSelectionFromRequest();
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

		private ContentItem PerformCopy(HttpContextBase context, ContentItem from, ContentItem to)
		{
			if (to == null)
				throw new InvalidOperationException("Cannot move to null");

			ContentItem newItem;
			var ex = engine.IntegrityManager.GetCopyException(from, to);
			if (ex != null)
				throw ex;

			if (!engine.SecurityManager.IsAuthorized(context.User, to, Security.Permission.Write))
				throw new UnauthorizedAccessException();

			newItem = engine.Persister.Copy(Selection.SelectedItem, to);

			if (engine.SecurityManager.IsAuthorized(context.User, to, Security.Permission.Publish))
				return newItem;

			engine.Resolve<StateChanger>().ChangeTo(newItem, ContentState.Draft);
			engine.Persister.Save(newItem);

			return newItem;
		}

		private IEnumerable<Node<TreeNode>> CreateChildren(HttpContextBase context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(Selection.SelectedItem);
			var filter = engine.EditManager.GetEditorFilter(context.User);

			var query = Query.From(Selection.SelectedItem);
			query.Interface = Interfaces.Managing;
			if (context.Request["pages"] != null)
				query.OnlyPages = Convert.ToBoolean(context.Request["pages"]);
			if (Selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge))
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
				HasChildren = adapter.HasChildren(item, filter),
				Expanded = false
			};
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}