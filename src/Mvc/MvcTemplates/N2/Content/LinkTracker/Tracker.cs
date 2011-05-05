using System;
using System.Linq;
using N2.Details;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using N2.Plugin;
using N2.Engine;

namespace N2.Edit.LinkTracker
{
	/// <summary>This class examines saved items and keeps tracks of links in html.</summary>
	[Service]
	public class Tracker : IAutoStart
	{
		public const string LinkDetailName = "TrackedLinks";
		Persistence.IPersister persister;
		Persistence.Finder.IItemFinder find;
		N2.Web.IUrlParser urlParser;
        N2.Web.IErrorNotifier errorHandler;

        public Tracker(Persistence.IPersister persister, Persistence.Finder.IItemFinder find, N2.Web.IUrlParser urlParser, N2.Web.IErrorNotifier errorHandler)
		{
			this.persister = persister;
			this.find = find;
			this.urlParser = urlParser;
            this.errorHandler = errorHandler;
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
			var referencedItems = FindLinkedObjects(item).ToList();
			DetailCollection links = item.GetDetailCollection(LinkDetailName, false);
			if (referencedItems.Count > 0)
			{
				if (links == null)
					links = item.GetDetailCollection(LinkDetailName, true);
				links.Replace(referencedItems);
			}
			else if (links != null && links.Count > 0)
			{
				links.Clear();
			}
		}

		/// <summary>Finds items linked by the supplied item. This method only finds links in html text to items within the site.</summary>
		/// <param name="item">The item to examine for links.</param>
		/// <returns>A list of items referenced in html text by the supplied item.</returns>
		public virtual IList<ContentItem> FindLinkedItems(ContentItem item)
		{
			return FindLinkedObjects(item).OfType<ContentItem>().ToList();
		}
		
		/// <summary>Finds items linked by the supplied item. This method only finds links in html text to items within the site.</summary>
		/// <param name="item">The item to examine for links.</param>
		/// <returns>A list of items referenced in html text by the supplied item.</returns>
		public virtual IEnumerable<object> FindLinkedObjects(ContentItem item)
		{
			HashSet<object> existing = new HashSet<object>();

			foreach (ContentDetail detail in item.Details)
			{
				if (detail.StringValue != null)
				{
					foreach (string link in FindLinks(detail.StringValue))
					{
                        if (string.IsNullOrEmpty(link))
                            continue;
                        else if (!(link.StartsWith("/") || link.StartsWith("~") || link.Contains("://")))
                            continue;

						object referencedItemOrLink = null;
						try
						{
							ContentItem referencedItem = urlParser.Parse(link);
							if (referencedItem != null && referencedItem.ID != 0)
							{
								referencedItemOrLink = referencedItem;
							}
						}
						catch (Exception ex)
						{
                            errorHandler.Notify(ex);
						}

						if (referencedItemOrLink == null)
							referencedItemOrLink = link;

						if (!existing.Contains(referencedItemOrLink))
						{
							existing.Add(referencedItemOrLink);
							yield return referencedItemOrLink;
						}
					}
				}
			}
		}

		/// <summary>Finds links in a html string using regular expressions.</summary>
		/// <param name="html">The html to search for links.</param>
		/// <returns>A list of link (a) href attributes in the supplied html string.</returns>
		public virtual IList<string> FindLinks(string html)
		{
			List<string> links = new List<string>();
			MatchCollection matches = Regex.Matches(html, "<a.*?href=[\"']*(?<link>[^\"'>]*).*?</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			foreach (Match m in matches)
			{
				if (m.Groups["link"].Success)
					links.Add(m.Groups["link"].Value);
			}
			if (links.Count == 0 && !html.Contains(Environment.NewLine) && (html.StartsWith("/") || Regex.IsMatch(html, "^\\w+://", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
				links.Add(html);
			return links;
		}

		/// <summary>Finds other whose links are beeing tracked by the link tracker. This doesn't include other links that might exist.</summary>
		/// <param name="item">The item is beeing referenced by the items we'd like to find.</param>
		/// <returns>A list of items linking to the supplied item.</returns>
		public virtual IList<ContentItem> FindReferrers(ContentItem item)
		{
			if (item.ID != 0)
				return find.Where.Detail(LinkDetailName).Eq(item)
					.Filters(new Collections.DuplicateFilter())
					.Select();
			else
				return find.Where.Detail(LinkDetailName).Like(item.Url)
					.Filters(new Collections.DuplicateFilter())
					.Select();
		}
		#endregion

		#region IStartable Members

		public void Start()
		{
			persister.ItemSaving += persister_ItemSaving;
		}

		public void Stop()
		{
			persister.ItemSaving -= persister_ItemSaving;
		}

		#endregion
	}
}
