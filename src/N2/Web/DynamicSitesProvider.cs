using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using N2.Collections;

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
	    private IHost host;
		private int recursionDepth = 2;
		#endregion

		#region Constructors
		public DynamicSitesProvider(Persistence.IPersister persister, IHost host)
		{
			if (persister == null) throw new ArgumentNullException("persister");
			if (host == null) throw new ArgumentNullException("host");

			this.persister = persister;
			this.host = host;
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
		public virtual IEnumerable<Site> GetSites()
		{
			if (RecursionDepth < 0) throw new N2Exception("The DynamicSitesProvider requires the RecursionDepth property to be at least 0");

            List<Site> foundSites = new List<Site>();
            
            foreach (Site site in host.Sites)
            {
                try
                {
                    ContentItem rootItem = persister.Get(site.RootItemID);
                    if (rootItem == null)
                        continue;

                    foreach (ISitesSource source in new RecursiveFinder().Find<ISitesSource>(rootItem, RecursionDepth))
                    {
                        foreach (Site s in source.GetSites())
                        {
                            if (!host.Sites.Contains(s))
                                foundSites.Add(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("DynamicSitesProvider.GetSites:" + ex);
                }
            }

		    return foundSites;
		}
		#endregion
	}
}
