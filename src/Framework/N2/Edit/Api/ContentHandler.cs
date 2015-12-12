using N2.Collections;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Collaboration;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Persistence.Sources;
using N2.Web;
using N2.Web.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using N2.Edit.Api;
using N2.Web.Parts;

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
			Authorize(context.User, Selection.SelectedItem);

			context.Response.SetNoCache();

			switch (context.Request.HttpMethod)
			{
				case "GET":
					switch (context.Request.PathInfo)
					{
						case "/search":
							WriteSearch(context);
							return;
						case "/translations":
							var translations = GetTranslations(context).ToList();
							context.Response.WriteJson(new { Translations = translations });
							return;
						case "/versions":
							var versions = GetVersions(context).ToList();
							context.Response.WriteJson(new { Versions = versions });
							return;
						case "/definitions":
							var definitions = GetDefinitionTemplateInfos(context);
							context.Response.WriteJson(new { Definitions = definitions });
							return;
						case "/templates":
							var templates = GetTemplateInfos(context);
							var wizards = GetwizardInfos(context);
							context.Response.WriteJson(new { Templates = templates, Wizards = wizards });
							return;
						case "/tokens":
							var tokens = GetTokens(context);
							context.Response.WriteJson(new { Tokens = tokens });
							return;
						case "":
						case "/":
						case "/children":
							var children = GetChildren(context).ToList();
							context.Response.WriteJson(new { Children = children, IsPaged = Selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge) });
							return;
						case "/branch":
							var branch = GetBranch(context);
							context.Response.WriteJson(new { Branch = branch });
							return;
						case "/tree":
							var tree = GetTree(context);
							context.Response.WriteJson(new { Tree = tree });
							return;
						case "/ancestors":
							var ancestors = GetAncestors(context);
							context.Response.WriteJson(new { Ancestors = ancestors });
							return;
						case "/parent":
							var parent = GetParent(context);
							context.Response.WriteJson(new { Parent = parent });
							return;
						case "/node":
							var node = GetNode(context);
							context.Response.WriteJson(new { Node = node });
							return;
						default:
							if (context.Request.PathInfo.StartsWith("/"))
							{
								int id;
								if (int.TryParse(context.Request.PathInfo.Trim('/'), out id))
								{
									var item = engine.Persister.Get(id);
									context.Response.WriteJson(item);
									return;
								}
							}
							break;
					}
					break;
				case "POST":
					EnsureValidSelection();
					switch (context.Request.PathInfo)
					{
						case "":
							Create(context);
							return;
						case "/update":
							Update(context);
							return;
						case "/sort":
						case "/move":
							Move(context, Selection.RequestValueAccessor);
							return;
						case "/organize":
							Organize(context, Selection);
							return;
						case "/copy":
							Copy(context, Selection.RequestValueAccessor);
							return;
						case "/delete":
							Delete(context);
							return;
						case "/publish":
							Publish(context);
							return;
						case "/unpublish":
							Unpublish(context);
							return;
						case "/schedule":
							Schedule(context);
							return;
						case "/autosave":
							Autosave(context);
							return;
						case "/discard":
							Discard(context);
							return;
					}
					break;
				case "DELETE":
					switch (context.Request.PathInfo)
					{
						case "/message":
							DeleteMessage(context);
							return;
						case "/":
						case "":
							EnsureValidSelection();
							Delete(context);
							return;
					}
					break;
				case "PUT":
					EnsureValidSelection();
					Update(context);
					return;
			}

			if (!TryExecuteExternalHandlers(context))
				throw new HttpException((int)HttpStatusCode.NotImplemented, "Not Implemented");
		}

		private void Discard(HttpContextBase context)
		{
			var published = Selection.SelectedItem.VersionOf.Value;
			if (published != null)
			{
				engine.Resolve<IVersionManager>().DeleteVersion(Selection.SelectedItem);
				context.Response.WriteJson(new
				{
					Discarded = engine.ResolveAdapter<NodeAdapter>(Selection.SelectedItem).GetTreeNode(Selection.SelectedItem),
					Node = engine.ResolveAdapter<NodeAdapter>(published).GetTreeNode(published)
				});
            }
			else if (Selection.SelectedItem.State <= ContentState.Draft)
			{
				var parent = Selection.SelectedItem.Parent ?? engine.UrlParser.StartPage;
				engine.Persister.Delete(Selection.SelectedItem);
				context.Response.WriteJson(new
				{
					Removed = engine.ResolveAdapter<NodeAdapter>(Selection.SelectedItem).GetTreeNode(Selection.SelectedItem),
                    Node = engine.ResolveAdapter<NodeAdapter>(parent).GetTreeNode(parent)
				});
			}
			else
			{
				throw new InvalidOperationException("Can only discard versions");
			}
		}

		private void Organize(HttpContextBase context, SelectionUtility selection)
		{
			var navigator = engine.Resolve<Navigator>();
			var persister = engine.Persister;
			var integrity = engine.IntegrityManager;
			var versions = engine.Resolve<IVersionManager>();
			var versionRepository = engine.Resolve<ContentVersionRepository>();

			string versionIndex = selection.RequestValueAccessor(PathData.VersionIndexQueryKey);
			string versionKey = selection.RequestValueAccessor(PathData.VersionKeyQueryKey);
            var path = PartsExtensions.EnsureDraft(versions, versionRepository, versionIndex, versionKey, selection.SelectedItem);
			ContentItem item = path.CurrentItem;
			ContentItem page = path.CurrentPage;

			item.ZoneName = selection.RequestValueAccessor("zone");

			var beforeItem = PartsExtensions.GetBeforeItem(navigator, selection.RequestValueAccessor, page);
			ContentItem parent;
			if (beforeItem != null)
			{
				parent = beforeItem.Parent;
				int newIndex = parent.Children.IndexOf(beforeItem);
				ThrowUnlessNull(integrity.GetMoveException(item, parent));
				Utility.Insert(item, parent, newIndex);
			}
			else
			{
				parent = PartsExtensions.GetBelowItem(navigator, selection.RequestValueAccessor, page);
				ThrowUnlessNull(integrity.GetMoveException(item, parent));
				Utility.Insert(item, parent, parent.Children.Count);
			}

			Utility.UpdateSortOrder(parent.Children);
			versionRepository.Save(page);
		}

		private void ThrowUnlessNull(Exception exception)
		{
			if (exception != null)
				throw exception;
		}

		private void Autosave(HttpContextBase context)
		{
			var selected = Selection.ParseSelectionFromRequest();
			if (selected == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "Not Found");

			var requestBody = context.GetOrDeserializeRequestStreamJsonDictionary<object>();
			var discriminator = EditExtensions.GetDiscriminator(context.Request);

			var versions = engine.Resolve<VersionManager>();
			ContentItem item;
			if (string.IsNullOrEmpty(discriminator))
			{
				item = selected;
				if (item.State != ContentState.Draft)
					item = versions.GetOrCreateDraft(item);

				Update(requestBody, item);

				if (item.ID == 0 && item.VersionOf.HasValue)
					versions.UpdateVersion(item);
				else
					engine.Persister.Save(item);
			}
			else
			{
				int id;
				if (requestBody.ContainsKey("ID") && (id = (int)requestBody["ID"]) != 0)
				{
					item = engine.Persister.Get(id);
				}
				else
				{
					item = engine.Resolve<IDefinitionManager>().GetDefinition(discriminator).CreateInstance(selected);
					item.State = ContentState.Draft;
				}

				Update(requestBody, item);

				if (item.ID == 0 && (item.VersionOf.HasValue || !item.IsPage))
					versions.UpdateVersion(item);
				else
					engine.Persister.Save(item);
			}

			context.Response.WriteJson(new
			{
				ID = item.VersionOf.ID ?? item.ID,
				VersionIndex = item.VersionIndex,
				Path = item.Path,
				PreviewUrl = item.Url,
				Permission = engine.ResolveAdapter<NodeAdapter>(item).GetMaximumPermission(item),
				Permissions = engine.SecurityManager.GetPermissions(context.User, item),
				Draft = new DraftInfo { ItemID = item.VersionOf.ID ?? item.ID, Saved = item.Updated, SavedBy = item.SavedBy, VersionIndex = item.VersionIndex },
				Node = engine.ResolveAdapter<NodeAdapter>(item.VersionOf.Value ?? item).GetTreeNode(item.VersionOf.Value ?? item)
			});
		}

		private void Update(IDictionary<string, object> requestBody, ContentItem item)
		{
			foreach (var kvp in requestBody)
				if (kvp.Key != "ID" && kvp.Key != "VersionIndex")
					item[kvp.Key] = kvp.Value;
		}

		private bool TryExecuteExternalHandlers(HttpContextBase context)
		{
			foreach (var handler in engine.Container.ResolveAll<ContentHandlerBase>())
			{
				if (handler.Handle(context))
					return true;
			}
			return false;
		}

		private List<TemplateInfo> GetwizardInfos(HttpContextBase context)
		{
			return engine.Container.ResolveAll<ITemplateInfoProvider>()
				.Where(tip => tip.Area == "Wizard")
				.SelectMany(tip => tip.GetTemplates())
				.ToList();
		}

		private List<TemplateInfo> GetTemplateInfos(HttpContextBase context)
		{
			var templates = GetDefinitions(context)
									 .SelectMany(d => engine.Resolve<ITemplateAggregator>().GetTemplates(d.ItemType))
									 .WhereAllowed(Selection.SelectedItem, context.Request["zoneName"], context.User, engine.Definitions, engine.SecurityManager)
									 .Select(d => new TemplateInfo(d)
									 {
										 EditUrl = engine.ManagementPaths.GetEditNewPageUrl(Selection.SelectedItem, d.Definition)
									 })
									 .ToList();
			return templates;
		}

		private List<TemplateInfo> GetDefinitionTemplateInfos(HttpContextBase context)
		{
			var definitions = GetDefinitions(context)
				.WhereAuthorized(engine.SecurityManager, context.User, Selection.SelectedItem)
				.Select(d => new TemplateInfo(d)
				{
					EditUrl = engine.ManagementPaths.GetEditNewPageUrl(Selection.SelectedItem, d)
				})
				.ToList();
			return definitions;
		}

		private void DeleteMessage(HttpContextBase context)
		{
			engine.Resolve<ManagementMessageCollector>().Delete(context.Request["source"], context.Request["id"]);
		}

		private void Authorize(IPrincipal user, ContentItem item)
		{
			if (!engine.SecurityManager.IsAuthorized(user, item, Security.Permission.Read))
				throw new UnauthorizedAccessException();
		}

		private void EnsureValidSelection()
		{
			if (Selection.ParseSelectionFromRequest() == null)
				throw new HttpException(404, "Not Found");
		}

		private Node<TreeNode> GetNode(HttpContextBase context)
		{
			return ApiExtensions.CreateNode(new HierarchyNode<ContentItem>(Selection.SelectedItem), engine.Resolve<IContentAdapterProvider>(), engine.EditManager.GetEditorFilter(context.User));
		}

		private Node<TreeNode> GetTree(HttpContextBase context)
		{
			var adapters = engine.Resolve<IContentAdapterProvider>();
			var selectedItem = Selection.SelectedItem;
			var filter = engine.EditManager.GetEditorFilter(context.User);
			int maxDepth;
			int.TryParse(context.Request["depth"], out maxDepth);
			var structure = ApiExtensions.BuildTreeStructure(filter, adapters, selectedItem, maxDepth);
			return ApiExtensions.CreateNode(structure, adapters, filter);
		}

		private TreeNode GetParent(HttpContextBase context)
		{
			var parent = Selection.SelectedItem.Parent;
			Authorize(context.User, parent);
			return engine.ResolveAdapter<NodeAdapter>(parent).GetTreeNode(parent);
		}

		private IEnumerable<TreeNode> GetAncestors(HttpContextBase context)
		{
			var root = Selection.ParseSelected(context.Request["root"]) ?? Selection.Traverse.RootPage;
			return Selection.Traverse.Ancestors(filter: engine.EditManager.GetEditorFilter(context.User), lastAncestor: root).Select(ci => engine.ResolveAdapter<NodeAdapter>(ci).GetTreeNode(ci)).ToList();
		}

		private Node<TreeNode> GetBranch(HttpContextBase context)
		{
			var root = Selection.ParseSelected(context.Request["root"]) ?? Selection.Traverse.RootPage;
			var selectedItem = Selection.SelectedItem;
			var filter = engine.EditManager.GetEditorFilter(context.User);
			var structure = ApiExtensions.BuildBranchStructure(filter, engine.Resolve<IContentAdapterProvider>(), selectedItem, root);
			return ApiExtensions.CreateNode(structure, engine.Resolve<IContentAdapterProvider>(), filter);
		}

		private IEnumerable<TokenDefinition> GetTokens(HttpContextBase context)
		{
			return engine.Resolve<TokenDefinitionFinder>().FindTokens();
		}

		private void Update(HttpContextBase context)
		{
			var item = Selection.ParseSelectionFromRequest();
			if (item == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "Not Found");

			var requestBody = context.GetOrDeserializeRequestStreamJsonDictionary<object>();
			foreach (var kvp in requestBody)
				item[kvp.Key] = kvp.Value;

			engine.Persister.Save(item);
		}

		private void Create(HttpContextBase context)
		{
			var parent = Selection.ParseSelectionFromRequest();
			if (parent == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "Not Found");

			var discriminator = context.Request["discriminator"];
			var definition = engine.Definitions.GetDefinition(discriminator);

			var item = engine.Resolve<ContentActivator>().CreateInstance(definition.ItemType, parent);

			var requestBody = context.GetOrDeserializeRequestStreamJsonDictionary<object>();
			foreach (var kvp in requestBody)
				item[kvp.Key] = kvp.Value;

			engine.Persister.Save(item);
		}

		private IEnumerable<ItemDefinition> GetDefinitions(HttpContextBase context)
		{
			var item = Selection.ParseSelectionFromRequest();
			if (item != null)
				return engine.Definitions.GetAllowedChildren(item, null).WhereAuthorized(engine.SecurityManager, context.User, item);
			else
				return engine.Definitions.GetDefinitions();
		}

		private IEnumerable<Node<TreeNode>> GetVersions(HttpContextBase context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(Selection.SelectedItem);
			var versions = engine.Resolve<IVersionManager>().GetVersionsOf(Selection.SelectedItem);

			foreach (var v in versions)
			{
				Node<TreeNode> node;
				try
				{
					node = new Node<TreeNode>(adapter.GetTreeNode(v.Content, allowDraft: false));
				}
				catch (Exception ex)
				{
					Logger.Error("Failure in GetVersions(HttpContextBase)", ex);
					node = new Node<TreeNode>(new TreeNode() { Title = "(invalid version)", ToolTip = ex.ToString() });
				}
				yield return node;
			}
		}

		private IEnumerable<Node<InterfaceMenuItem>> GetTranslations(HttpContextBase context)
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

			using (var tx = engine.Persister.Repository.BeginTransaction())
			{
				if (!string.IsNullOrEmpty(request("before")))
				{
					var before = engine.Resolve<Navigator>().Navigate(request("before"));

					PerformMoveChecks(context, from, before.Parent);

					if (!string.IsNullOrEmpty(request("zone")))
						from.ZoneName = request("zone");
					sorter.MoveTo(from, NodePosition.Before, before);
				}
				else
				{
					var to = engine.Resolve<Navigator>().Navigate(request("to"));

					PerformMoveChecks(context, from, to);

					if (!string.IsNullOrEmpty(request("zone")))
						from.ZoneName = request("zone");
					sorter.MoveTo(from, to);
					engine.Resolve<ITrashHandler>().HandleMoved(from);
				}
				engine.Persister.Save(from);
				tx.Commit();
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

		private IEnumerable<Node<TreeNode>> GetChildren(HttpContextBase context)
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
				.Select(c => GetNode(c, query));
		}

		private Node<TreeNode> GetNode(ContentItem item, Query query)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(item);
			return new Node<TreeNode>
			{
				Current = adapter.GetTreeNode(item),
				Children = new Node<TreeNode>[0],
				HasChildren = adapter.HasChildren(new Query { Parent = item, Filter = query.Filter, Interface = query.Interface, OnlyPages = query.OnlyPages }),
				Expanded = false
			};
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
