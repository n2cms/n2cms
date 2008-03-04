using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace N2.Web
{
	/// <summary>
	/// Finds available sites by looking for content items implementing the 
	/// <see cref="ISitesSource"/> interface.
	/// </summary>
	public class DynamicSitesProvider : N2.Web.ISitesProvider
	{
		#region Private Fields
		private Persistence.IPersister persister;
		private int rootItemID;
		private int recursionDepth = 1;
		#endregion

		#region Constructors
		public DynamicSitesProvider(Persistence.IPersister persister, int rootItemID)
		{
			this.persister = persister;
			this.rootItemID = rootItemID;
		}

		public DynamicSitesProvider(Persistence.IPersister persister, Site defaultSite)
		{
			this.persister = persister;
			this.rootItemID = defaultSite.RootItemID;
		} 
		#endregion

		#region Properties
		/// <summary>Gets or sets the number of levels to look for site sources. 0 means root item only. 1 also includes the root item's child pages, etc.</summary>
		public int RecursionDepth
		{
			get { return recursionDepth; }
			set { recursionDepth = value; }
		}
		#endregion

		#region Methods
		public virtual ICollection<Site> GetSites()
		{
			//try
			//{
				if (RecursionDepth < 0)
					throw new N2Exception("The DynamicSitesProvider requires the RecursionDepth property to be at least 0");

				List<Site> sites = new List<Site>();
				ContentItem rootItem = persister.Get(rootItemID);

				AppendSitesRecursive(sites, rootItem, 0);

				return sites;
			//}
			//catch (Exception ex)
			//{
			//    //TODO: check out
			//    // too bad: probably some serious error but we can't throw here because this might screw up the installation
			//    Trace.TraceError(ex.ToString());
			//    return new Site[0];
			//}
		}

		protected virtual void AppendSitesRecursive(List<Site> sites, ContentItem currentItem, int currentDepth)
		{
			if (currentDepth <= RecursionDepth)
			{
				AppendSites(sites, currentItem as ISitesSource);
				foreach (ContentItem childItem in currentItem.Children)
					AppendSitesRecursive(sites, childItem, currentDepth + 1);
			}
		}

		protected virtual void AppendSites(List<Site> sites, ISitesSource sitesSource)
		{
			if (sitesSource != null)
				foreach (Site site in sitesSource.GetSites())
					sites.Add(site);
		} 
		#endregion
	}
}
