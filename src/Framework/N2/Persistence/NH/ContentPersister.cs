#region License

/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */

#endregion

using System.Linq;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence.Finder;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
    /// <summary>
    /// A wrapper for NHibernate persistence functionality.
    /// </summary>
    [Service(typeof(IPersister))]
    public class ContentPersister : ContentPersisterBase, IPersister
    {
        private readonly INHRepository<int, ContentDetail> linkRepository;

        /// <summary>Creates a new instance of the NHPersistenceManager.</summary>
        public ContentPersister(IRepository<int, ContentItem> itemRepository, INHRepository<int, ContentDetail> linkRepository,
                                IItemFinder finder)
            : base(itemRepository, finder)
        {
            this.linkRepository = linkRepository;
        }

        protected override void SaveAction(ContentItem item)
        {
            if (item is IActiveContent)
            {
                (item as IActiveContent).Save();
            }
            else
            {
                using (ITransaction transaction = itemRepository.BeginTransaction())
                {
                    if (item.VersionOf == null)
                        item.Updated = Utility.CurrentTime();
                    if (string.IsNullOrEmpty(item.Name))
                        item.Name = null;

                    item.AddTo(item.Parent);
                    ensureSortOrder(item);

                    itemRepository.SaveOrUpdate(item);
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        item.Name = item.ID.ToString();
                        itemRepository.Save(item);
                    }

                    transaction.Commit();
                }
            }
        }

        private void ensureSortOrder(ContentItem unsavedItem)
        {
            var parent = unsavedItem.Parent;
            if (parent != null)
            {
                foreach (SortChildrenAttribute attribute in parent.GetContentType().GetCustomAttributes(typeof(SortChildrenAttribute), true))
                {
                    foreach (ContentItem updatedItem in attribute.ReorderChildren(parent))
                    {
                        itemRepository.SaveOrUpdate(updatedItem);
                    }
                }
            }
        }

        protected override void DeleteAction(ContentItem itemNoMore)
        {
            if (itemNoMore is IActiveContent)
            {
                TraceInformation("ContentPersister.DeleteAction " + itemNoMore + " is IActiveContent");
                (itemNoMore as IActiveContent).Delete();
            }
            else
            {
                using (ITransaction transaction = itemRepository.BeginTransaction())
                {
                    deleteReferencesRecursive(itemNoMore);

                    DeleteRecursive(itemNoMore, itemNoMore);

                    transaction.Commit();
                }
            }
        }

        private void deleteReferencesRecursive(ContentItem itemNoMore)
        {
            string itemTrail = Utility.GetTrail(itemNoMore);
            var inboundLinks = Find.EnumerateChildren(itemNoMore, true, false)
                .SelectMany(i => linkRepository.FindAll(Expression.Eq("LinkedItem", i), Expression.Eq("ValueTypeKey", ContentDetail.TypeKeys.LinkType)))
                .Where(l => !Utility.GetTrail(l.EnclosingItem).StartsWith(itemTrail))
                .ToList();

            TraceInformation("ContentPersister.DeleteReferencesRecursive " + inboundLinks.Count + " of " + itemNoMore);

            foreach (ContentDetail link in inboundLinks)
            {
                linkRepository.Delete(link);
                link.AddTo((DetailCollection)null);
            }
            linkRepository.Flush();
        }

        protected override ContentItem MoveAction(ContentItem source, ContentItem destination)
        {
            if (source is IActiveContent)
            {
                TraceInformation("ContentPersister.MoveAction " + source + " (is IActiveContent) to " + destination);
                (source as IActiveContent).MoveTo(destination);
            }
            else
            {
                using (ITransaction transaction = itemRepository.BeginTransaction())
                {
                    TraceInformation("ContentPersister.MoveAction " + source + " to " + destination);
                    source.AddTo(destination);
                    //source.AncestralTrail = null;
                    Save(source);
                    transaction.Commit();
                }
            }
            return null;
        }
    }
}