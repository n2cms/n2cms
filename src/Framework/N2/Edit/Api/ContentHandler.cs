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
                            break;
                        case "/translations":
                            var translations = GetTranslations(context).ToList();
                            context.Response.WriteJson(new { Translations = translations });
                            break;
                        case "/versions":
                            var versions = GetVersions(context).ToList();
                            context.Response.WriteJson(new { Versions = versions });
							break;
						case "/definitions":
							var definitions = GetDefinitionTemplateInfos(context);
							context.Response.WriteJson(new { Definitions = definitions });
							break;
						case "/templates":
							var templates = GetTemplateInfos(context);
							var wizards = GetwizardInfos(context);
							context.Response.WriteJson(new { Templates = templates, Wizards = wizards });
							break;
                        case "/tokens":
                            var tokens = GetTokens(context);
                            context.Response.WriteJson(new { Tokens = tokens });
                            break;
                        case "/children":
							var children = GetChildren(context).ToList();
                            context.Response.WriteJson(new { Children = children, IsPaged = Selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge) });
                            break;
                        case "/branch":
							var branch = GetBranch(context);
                            context.Response.WriteJson(new { Branch = branch });
                            break;
                        case "/tree":
							var tree = GetTree(context);
                            context.Response.WriteJson(new { Tree = tree });
                            break;
                        case "/ancestors":
							var ancestors = GetAncestors(context);
                            context.Response.WriteJson(new { Ancestors = ancestors });
                            break;
                        case "/parent":
							var parent = GetParent(context);
                            context.Response.WriteJson(new { Parent = parent });
                            break;
                        case "/node":
							var node = GetNode(context);
                            context.Response.WriteJson(new { Node = node });
                            break;
                        default:
                            if (string.IsNullOrEmpty(context.Request.PathInfo))
                            {
                                context.Response.WriteJson(new { Children = GetChildren(context).ToList(), IsPaged = Selection.SelectedItem.ChildState.IsAny(CollectionState.IsLarge) });
                            }
                            else
                            {
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
                                throw new HttpException((int)HttpStatusCode.NotImplemented, "Not Implemented");
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
                            break;
                        case "/update":
                            Update(context);
                            break;
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
					switch (context.Request.PathInfo)
					{
						case "/message":
							DeleteMessage(context);
							break;
						default:
							EnsureValidSelection();
							Delete(context);
							break;
					}
                    break;
                case "PUT":
                    EnsureValidSelection();
                    Update(context);
                    break;
            }
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

                sorter.MoveTo(from, NodePosition.Before, before);
            }
            else
            {
                var to = engine.Resolve<Navigator>().Navigate(request("to"));

                PerformMoveChecks(context, from, to);

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
