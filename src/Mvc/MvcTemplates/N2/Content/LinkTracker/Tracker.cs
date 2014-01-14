using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using N2.Details;
using N2.Engine;
using N2.Plugin;
using N2.Persistence;
using N2.Definitions;
using N2.Collections;

namespace N2.Edit.LinkTracker
{
    /// <summary>This class examines saved items and keeps tracks of links in html.</summary>
    [Service]
    public class Tracker : IAutoStart
    {
        public const string LinkDetailName = "TrackedLinks";

        IRepository<ContentItem> repository;
        N2.Web.IUrlParser urlParser;
        N2.Web.IErrorNotifier errorHandler;
        Logger<Tracker> logger;

        public Tracker(Persistence.IPersister persister, N2.Web.IUrlParser urlParser, ConnectionMonitor connections, N2.Web.IErrorNotifier errorHandler, Configuration.EditSection config)
        {
            this.repository = persister.Repository;
            this.urlParser = urlParser;
            this.errorHandler = errorHandler;

            if (config.LinkTracker.Enabled)
            {
                connections.Online += delegate
                {
                    persister.ItemSaving += persister_ItemSaving;
                };
                connections.Offline += delegate
                {
                    persister.ItemSaving -= persister_ItemSaving;
                };
            }
        }

        void persister_ItemSaving(object sender, CancellableItemEventArgs e)
        {
            try
            {
                OnTrackingLinks(e.AffectedItem);
            }
            catch(Exception ex)
            {
                errorHandler.Notify(ex);
            }
        }

        #region Methods
        /// <summary>Is invoked when an item is beeing saved.</summary>
        /// <param name="item">The item that is beeing saved.</param>
        protected virtual void OnTrackingLinks(ContentItem item)
        {
            if (item is ISystemNode)
            {
                logger.DebugFormat("Not updating {0} due to ISystemNode", item);
                return;
            }
            UpdateLinks(item, true);
        }

        public virtual IEnumerable<ContentItem> UpdateLinks(ContentItem item, bool recursivelyUpdateParts = false)
        {
            return UpdateLinksInternal(item, recursivelyUpdateParts).ToList();
        }

        private IEnumerable<ContentItem> UpdateLinksInternal(ContentItem item, bool recursive)
        {
            logger.DebugFormat("Updating link: {0}", item);

            if (recursive)
                foreach (var part in item.Children.FindParts())
                    foreach (var updatedItem in UpdateLinksInternal(part, recursive))
                        yield return updatedItem;

            var referencedItems = FindLinkedObjects(item).ToList();
            DetailCollection links = item.GetDetailCollection(LinkDetailName, false);
            if (links == null && referencedItems.Count == 0)
            {
                logger.Debug("Exiting due to no links and none to update");
                yield break;
            }

            if (links == null)
                links = item.GetDetailCollection(LinkDetailName, true);

            logger.DebugFormat("Updating {0} links to {1} existing", referencedItems.Count, links.Count);

            // replace existing items
            for (int i = 0; i < referencedItems.Count && i < links.Count; i++)
            {
                var extracted = referencedItems[i];
                var stored = links.Details[i];
                if (stored.StringValue == extracted.StringValue && stored.Meta == extracted.Meta && stored.IntValue == extracted.IntValue && stored.BoolValue == extracted.BoolValue)
                    // this prevents clearing item references to children when moving hierarchies
                    continue;
                stored.Extract(extracted);
            }
            // add any new items
            for (int i = links.Details.Count; i < referencedItems.Count; i++)
            {
                links.Add(referencedItems[i]);
            }
            // remove any extra items
            while (links.Count > referencedItems.Count)
            {
                links.RemoveAt(links.Count - 1);
            }

            yield return item;
        }

        /// <summary>Finds items linked by the supplied item. This method only finds links in html text to items within the site.</summary>
        /// <param name="item">The item to examine for links.</param>
        /// <returns>A list of items referenced in html text by the supplied item.</returns>
        public virtual IList<ContentItem> FindLinkedItems(ContentItem item)
        {
            return FindLinkedObjects(item).Select(cd => cd.LinkedItem.Value).Where(i => i != null).ToList();
        }

        /// <summary>Finds items linked by the supplied item. This method only finds links in html text to items within the site.</summary>
        /// <param name="item">The item to examine for links.</param>
        /// <returns>A list of items referenced in html text by the supplied item.</returns>
        public virtual IEnumerable<ContentDetail> FindLinkedObjects(ContentItem item)
        {
            HashSet<object> existing = new HashSet<object>();

            foreach (ContentDetail detail in item.Details)
            {
                if (detail.StringValue != null)
                {
                    foreach (var d in FindLinks(detail.StringValue))
                    {
                        string link = d.StringValue;
                        if (string.IsNullOrEmpty(link))
                            continue;
                        if (!(link.StartsWith("/") || link.StartsWith("~") || link.Contains("://")))
                            continue;

                        d.Meta = detail.Name;

                        try
                        {
                            ContentItem referencedItem = urlParser.Parse(link);
                            if (referencedItem != null && referencedItem.ID != 0)
                            {
                                d.LinkedItem = referencedItem;
                            }
                        }
                        catch (Exception ex)
                        {
                            errorHandler.Notify(ex);
                        }

                        var reference = (object)d.LinkedItem ?? d.StringValue;
                        if (existing.Contains(reference))
                            d.BoolValue = true;
                        else
                            existing.Add(reference);

                        yield return d;
                    }
                }
            }
        }

        static Regex linkExpression = new Regex(@"<(?<tagname>(a)|img)\b[^>]*?\b(?<urltype>(?(1)href|src))\s*=\s*(?:""(?<link>(?:\\""|[^""])*)""|'(?<link>(?:\\'|[^'])*)')", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>Finds links in a html string using regular expressions.</summary>
        /// <param name="html">The html to search for links.</param>
        /// <returns>A list of link (a) href attributes in the supplied html string.</returns>
        public virtual IList<ContentDetail> FindLinks(string html)
        {
            List<ContentDetail> links = new List<ContentDetail>();
            MatchCollection matches = linkExpression.Matches(html);
            foreach (Match m in matches)
            {
                var g = m.Groups["link"];
                if (g.Success)
                    links.Add(ContentDetail.Multi(LinkDetailName, true, g.Index, null, null, g.Value, null, null));
            }
            if (links.Count == 0 && !html.Contains(Environment.NewLine) && (html.StartsWith("/") || Regex.IsMatch(html, "^\\w+://", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
                links.Add(ContentDetail.Multi(LinkDetailName, false, 0, null, null, html, null, null));
            return links;
        }

        /// <summary>Finds other whose links are beeing tracked by the link tracker. This doesn't include other links that might exist.</summary>
        /// <param name="item">The item is beeing referenced by the items we'd like to find.</param>
        /// <returns>A list of items linking to the supplied item.</returns>
        public virtual IEnumerable<ContentItem> FindReferrers(ContentItem item)
        {
            if (item.ID == 0)
                return FindReferrers(item.Url);

            return repository.Find(Parameter.Equal(Tracker.LinkDetailName, item).Detail() | Parameter.Equal(Tracker.LinkDetailName, item.Url).Detail())
                .Distinct();
        }

        public virtual IEnumerable<ContentItem> FindReferrers(string url)
        {
            return repository.Find(Parameter.Like(Tracker.LinkDetailName, url + "%").Detail())
                .Distinct();
        }

        #endregion

        #region IStartable Members

        public void Start()
        {
        }

        public void Stop()
        {
        }

        #endregion

        public virtual void UpdateReferencesTo(ContentItem targetItem, string oldUrl = null, bool isRenamingDirectory = false)
        {
            logger.InfoFormat("Updating references to {0} from old url '{1}'. directory = {2}", targetItem, oldUrl, isRenamingDirectory);

            var newUrl = targetItem.Url;
            using (var tx = repository.BeginTransaction())
            {
                var itemsWithReferencesToTarget = oldUrl == null ? 
                    FindReferrers(targetItem).ToList() : 
                    FindReferrers(oldUrl).ToList();

                foreach (var referrer in itemsWithReferencesToTarget)
                {
                    var trackerCollecetion = referrer.GetDetailCollection(Tracker.LinkDetailName, false);
                    if (trackerCollecetion == null || trackerCollecetion.Details.Count == 0)
                        continue;

                    var trackerDetails = trackerCollecetion.Details
                        .Where(d => d.IntValue.HasValue)
                        .OrderBy(d => d.IntValue.Value)
                        .ToList();

                    logger.DebugFormat("Updating links on {0}. Details = {1}", referrer, trackerDetails.Count);

                    for (int i = 0; i < trackerDetails.Count; i++)
                    {
                        var detail = trackerDetails[i];

                        var name = detail.Meta;

                        if (name == null || detail.StringValue == null)
                            // don't update legacy links
                            continue;
                        if (!detail.LinkedItem.HasValue && targetItem.ID != 0)
                        {
                            logger.DebugFormat("Skipping due to null linked item and nonempty target {0}", targetItem);
                            continue;
                        }
                        else if (detail.LinkedItem.HasValue && detail.LinkedItem.ID != targetItem.ID)
                        {
                            // don't update other links
                            logger.DebugFormat("Skipping due to other linked item: {0} != {1}", detail.LinkedItem, targetItem);
                            continue;
                        }
                        // case 1: directory rename
                        // oldUrl:      /upl/x/y/
                        // StringVal:   /upl/x/y/hej.jpg
                        // item.Url     /upl/x/y2/
                        else if (oldUrl != null && !detail.StringValue.StartsWith(oldUrl, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // don't update other images
                            logger.DebugFormat("Skipping due to other url: '{0}' != '{1}'", detail.StringValue, oldUrl);
                            continue;
                        }

                        if (isRenamingDirectory)
                        {
                            newUrl = targetItem.Url + detail.StringValue.Substring(oldUrl.Length);
                        }

                        Update(referrer, detail, name, newUrl, trackerDetails.Skip(i + 1).ToList());
                    }

                    repository.SaveOrUpdate(referrer);
                }

                tx.Commit();
            }
        }

        private void Update(ContentItem referrer, ContentDetail detail, string name, string newUrl, IEnumerable<ContentDetail> subsequentReferences)
        {
            int startIndex = detail.IntValue.Value;
            string oldUrl = detail.StringValue;
            string html = referrer[name] as string;
            if (html == null || html.Length < (startIndex + oldUrl.Length) || html.Substring(startIndex, oldUrl.Length) != oldUrl)
                return;

            // change the html
            referrer[name] = html.Substring(0, startIndex)
                + newUrl
                + html.Substring(startIndex + detail.StringValue.Length);

            // and update the reference
            detail.StringValue = newUrl;

            logger.DebugFormat("Updating links on {0}. Detail = {1}, Start index = {2}, old url = '{3}', new url = '{4}'", referrer, name, startIndex, oldUrl, newUrl);

            // adapt other reference indexes to the new string length
            foreach (var subsequent in subsequentReferences.Where(d => name.Equals(d.Meta)))
            {
                subsequent.IntValue = subsequent.IntValue.Value - oldUrl.Length + newUrl.Length;
            }
        }
    }
}
