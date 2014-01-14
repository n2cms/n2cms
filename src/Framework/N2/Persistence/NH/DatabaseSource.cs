using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using System.Diagnostics;
using N2.Collections;
using N2.Engine;

namespace N2.Persistence.Sources
{
    [ContentSource]
    public class DatabaseSource : SourceBase
    {
        private IHost host;
        private IContentItemRepository repository;
        Logger<DatabaseSource> logger;

        public DatabaseSource(IHost host, IContentItemRepository repository)
        {
            this.host = host;
            this.repository = repository;
        }

        public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
        {
            IEnumerable<ContentItem> items;
            if (!query.OnlyPages.HasValue)
                items = query.Parent.Children;
            else if (query.OnlyPages.Value)
                items = query.Parent.Children.FindPages();
            else
                items = query.Parent.Children.FindParts();

            if (query.Filter != null)
                items = items.Where(query.Filter);

            if (query.Limit != null)
                items = items.Skip(query.Limit.Skip).Take(query.Limit.Take);

            return previousChildren.Union(items);
        }

        public override bool HasChildren(Query query)
        {
            if (query.Parent.ChildState == CollectionState.IsEmpty)
                return false;

            bool onlyPages = query.OnlyPages ?? false;

            if (query.Parent.ChildState.IsAny(CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsHiddenPublicPages))
                return true;
            if (!onlyPages && query.Parent.ChildState.IsAny(CollectionState.ContainsPublicParts))
                return true;
            if (query.Filter != null && query.Parent.ChildState.IsAny(CollectionState.ContainsHiddenSecuredPages | CollectionState.ContainsVisibleSecuredPages))
                return true;
            if (!onlyPages && query.Filter != null && query.Parent.ChildState.IsAny(CollectionState.ContainsSecuredParts))
                return true;

            return base.HasChildren(query);
        }

        public override bool IsProvidedBy(ContentItem item)
        {
            return true;
        }

        public override PathData ResolvePath(string path)
        {
            var root = repository.Get(host.CurrentSite.RootItemID);
            return ResolvePath(root, path);
        }

        public override PathData ResolvePath(ContentItem startingPoint, string path)
        {
            return startingPoint.FindPath(path);
        }




        public override ContentItem Get(object id)
        {
            return repository.Get(id);
        }

        public override void Save(ContentItem item)
        {
            using (var tx = repository.BeginTransaction())
            {
                // update updated date unless it's a version being saved
                if (!item.VersionOf.HasValue)
                    item.Updated = Utility.CurrentTime();
                // empty string names not allowed, null is replaced with item id
                if (string.IsNullOrEmpty(item.Name))
                    item.Name = null;

                item.AddTo(item.Parent);
                
                // make sure the ordering is the same next time these siblings are loaded
                var unsavedItems = item.Parent.EnsureChildrenSortOrder();
                foreach (var itemToSave in unsavedItems.Union(new [] { item }))
                {
                    repository.SaveOrUpdate(itemToSave);
                }

                // ensure a name, fallback to id
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = item.ID.ToString();
                    repository.SaveOrUpdate(item);
                }

                tx.Commit();
            }
        }

        public override void Delete(ContentItem item)
        {
            using (var tx = repository.BeginTransaction())
            {
                // delete inbound references, these would cuase fk violation in the database
                repository.RemoveReferencesToRecursive(item);

                DeleteRecursive(item);

                tx.Commit();
            }
        }

        private void DeleteRecursive(ContentItem itemToDelete)
        {
            using (logger.Indent())
            {
                foreach (ContentItem child in itemToDelete.Children.ToList())
                    DeleteRecursive(child);
            }

            itemToDelete.AddTo(null);

            logger.InfoFormat("Deleting {0}", itemToDelete);
            repository.Delete(itemToDelete);
        }

        public override ContentItem Move(ContentItem source, ContentItem destination)
        {
            using (var tx = repository.BeginTransaction())
            {
                logger.Info("ContentPersister.MoveAction " + source + " to " + destination);
                source.AddTo(destination);
                foreach (var descendant in UpdateAncestralTrailRecursive(source, destination))
                {
                    repository.SaveOrUpdate(descendant);
                }
                Save(source);
                tx.Commit();
            }
            return source;
        }

        private IEnumerable<ContentItem> UpdateAncestralTrailRecursive(ContentItem source, ContentItem destination)
        {
            source.AncestralTrail = destination.GetTrail();
            foreach (var child in source.Children)
            {
                yield return child;
                foreach (var descendant in UpdateAncestralTrailRecursive(child, source))
                    yield return descendant;
            }
        }

        public override ContentItem Copy(ContentItem source, ContentItem destination)
        {
            logger.Info("ContentPersister.Copy " + source + " to " + destination);
            ContentItem cloned = source.Clone(includeChildren:true);
            if (cloned.Name == source.ID.ToString())
                cloned.Name = null;
            cloned.Parent = destination;

            Save(cloned);
            foreach (var descendant in Find.EnumerateChildren(cloned).ToArray())
                Save(descendant);

            return cloned;
        }
    }
}
