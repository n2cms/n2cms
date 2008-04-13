using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using N2.Persistence;

namespace N2.Web
{
	/// <summary>
	/// Parses urls in a multiple host environment.
	/// </summary>
	public class MultipleHostsUrlParser : UrlParser
	{
		#region Private Fields

		private IList<Site> sites = new List<Site>();

		#endregion

		#region Constructors

		public MultipleHostsUrlParser(IPersister persister, IWebContext webContext, IItemNotifier notifier, int rootItemID,
		                              ISitesProvider sitesProvider)
			: base(persister, webContext, notifier, rootItemID)
		{
			foreach (Site s in sitesProvider.GetSites())
				Sites.Add(s);
		}

		public MultipleHostsUrlParser(IPersister persister, IWebContext webContext, IItemNotifier notifier, Site defaultSite,
		                              ISitesProvider sitesProvider)
			: base(persister, webContext, notifier, defaultSite)
		{
			foreach (Site s in sitesProvider.GetSites())
				Sites.Add(s);
		}

		#endregion

		#region Properties

		public IList<Site> Sites
		{
			get { return sites; }
			set { sites = value; }
		}

		public override Site CurrentSite
		{
			get { return GetSite(WebContext.Host) ?? base.CurrentSite; }
		}

		#endregion

		#region Methods

		public override ContentItem Parse(string url)
		{
			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.Parse(url);
			else
			{
				string host = GetHost(url);
				Site site = GetSite(host);
				if (site != null)
					return TryLoadingFromQueryString(url) ?? Parse(Persister.Get(site.StartPageID), GetPathAndQuery(url));
				else
					return TryLoadingFromQueryString(url);
			}
		}

		public override ContentItem ParsePage(string url)
		{
			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.ParsePage(url);
			else
			{
				string host = GetHost(url);
				Site site = GetSite(host);
				if (site != null)
					return TryLoadingFromQueryString(url, "item", "page") ?? Parse(Persister.Get(site.StartPageID), GetPathAndQuery(url));
				else
					return TryLoadingFromQueryString(url, "page");
			}
		}

		public override string BuildUrl(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			ContentItem current = item;
			string url = string.Empty;

			// Walk the item's parent items to compute it's url
			do
			{
				if (IsRootOrStartPage(current))
					break;
				if (current.IsPage)
					url = "/" + current.Name + url;
				current = current.Parent;
			} while (current != null);

			if (current.ID == CurrentSite.StartPageID)
				return ToAbsolute(url, item);
			else
			{
				foreach (Site site in Sites)
					if (current.ID == site.StartPageID)
						return "http://" + site.Host + ToAbsolute(url, item);
				return "http://" + DefaultSite.Host + ToAbsolute(url, item);
			}
		}

		public override bool IsRootOrStartPage(ContentItem item)
		{
			foreach (Site site in Sites)
				if (item.ID == site.StartPageID)
					return true;
			return base.IsRootOrStartPage(item);
		}

		public virtual Site GetSite(string host)
		{
			foreach (Site site in Sites)
				if (site.Host == host)
					return site;
			return null;
		}

		public virtual string GetHost(string url)
		{
			Match m = Regex.Match(url, @"^\w+?://([^/]+)", RegexOptions.Compiled);
			if (m != null && m.Groups.Count > 1)
				return m.Groups[1].Value.ToLower();
			return null;
		}

		#endregion
	}
}