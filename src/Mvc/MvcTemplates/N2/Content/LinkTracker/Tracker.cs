using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using N2.Details;
using N2.Engine;
using N2.Plugin;
using N2.Persistence;

namespace N2.Edit.LinkTracker
{
	/// <summary>This class examines saved items and keeps tracks of links in html.</summary>
	[Service]
	public class Tracker : IAutoStart
	{
		public const string LinkDetailName = "TrackedLinks";
		Persistence.IPersister persister;
		IRepository<ContentDetail> detailRepository;
		N2.Web.IUrlParser urlParser;
        N2.Web.IErrorNotifier errorHandler;

        public Tracker(Persistence.IPersister persister, IRepository<ContentDetail> detailRepository, N2.Web.IUrlParser urlParser, ConnectionMonitor connections, N2.Web.IErrorNotifier errorHandler, Configuration.EditSection config)
		{
			this.persister = persister;
			this.detailRepository = detailRepository;
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
			UpdateLinks(item);
		}

		public virtual void UpdateLinks(ContentItem item)
		{
			var referencedItems = FindLinkedObjects(item).ToList();
			DetailCollection links = item.GetDetailCollection(LinkDetailName, false);
			if (links == null && referencedItems.Count == 0)
				return;

			if (links == null)
				links = item.GetDetailCollection(LinkDetailName, true);

			// replace existing items
			for (int i = 0; i < referencedItems.Count && i < links.Count; i++)
			{
				if (links.Details[i].StringValue == referencedItems[i].StringValue)
					// this prevents clearing item references to children when moving hierarchies
					continue;
				links.Details[i].Extract(referencedItems[i]);
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
		}

		/// <summary>Finds items linked by the supplied item. This method only finds links in html text to items within the site.</summary>
		/// <param name="item">The item to examine for links.</param>
		/// <returns>A list of items referenced in html text by the supplied item.</returns>
		public virtual IList<ContentItem> FindLinkedItems(ContentItem item)
		{
			return FindLinkedObjects(item).Select(cd => cd.LinkedItem).Where(i => i != null).ToList();
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

		/// <summary>Finds links in a html string using regular expressions.</summary>
		/// <param name="html">The html to search for links.</param>
		/// <returns>A list of link (a) href attributes in the supplied html string.</returns>
		public virtual IList<ContentDetail> FindLinks(string html)
		{
			List<ContentDetail> links = new List<ContentDetail>();
			MatchCollection matches = Regex.Matches(html, "<a.*?href=[\"']*(?<link>[^\"'>]*).*?</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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
			{
				return FindReferrers(item.Url);
			}
			else
			{
				return detailRepository.Find(new Parameter("Name", Tracker.LinkDetailName), new Parameter("LinkedItem.ID", item.ID))
					.Select(d => d.EnclosingItem)
					.Distinct();
			}
			//if (item.ID != 0)
			//    return find.Where.Detail(LinkDetailName).Eq(item)
			//        .Filters(new Collections.DuplicateFilter())
			//        .Select();
			//else
			//    return find.Where.Detail(LinkDetailName).Like(item.Url)
			//        .Filters(new Collections.DuplicateFilter())
			//        .Select();
		}

		private IEnumerable<ContentItem> FindReferrers(string url)
		{
			return detailRepository.Find(new Parameter("Name", Tracker.LinkDetailName), new Parameter("StringValue", url))
				.Select(d => d.EnclosingItem)
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

		public virtual void UpdateReferencesTo(ContentItem targetItem)
		{
			var newUrl = targetItem.Url;
			var repository = persister.Repository;
			using (var tx = repository.BeginTransaction())
			{
				var itemsWithReferencesToTarget = FindReferrers(targetItem);
				foreach (var referrer in itemsWithReferencesToTarget)
				{
					var trackerCollecetion = referrer.GetDetailCollection(Tracker.LinkDetailName, false);
					if (trackerCollecetion == null || trackerCollecetion.Details.Count == 0)
						continue;

					var trackerDetails = trackerCollecetion.Details
						.Where(d => d.IntValue.HasValue)
						.OrderBy(d => d.IntValue.Value)
						.ToList();

					for (int i = 0; i < trackerDetails.Count; i++)
					{
						var detail = trackerDetails[i];

						var name = detail.Meta;
						if (name == null || detail.StringValue == null)
							// don't update legacy links
							continue;

						if (detail.LinkedItem == null)
							continue;
						if (detail.LinkedItem.ID != targetItem.ID)
						{
							// don't update other links
							continue;
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

			// adapt other reference indexes to the new string length
			foreach (var subsequent in subsequentReferences.Where(d => name.Equals(d.Meta)))
			{
				subsequent.IntValue = subsequent.IntValue.Value - oldUrl.Length + newUrl.Length;
			}
		}
	}
}
