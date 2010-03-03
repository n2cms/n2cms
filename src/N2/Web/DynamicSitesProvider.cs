using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using N2.Collections;
using N2.Configuration;
using N2.Engine;

namespace N2.Web
{
	/// <summary>
	/// Finds available sites by looking for content items implementing the 
	/// <see cref="ISitesSource"/> interface.
	/// </summary>
	[Service(typeof(ISitesProvider))]
	public class DynamicSitesProvider : N2.Web.ISitesProvider
	{
		#region Private Fields
		private Persistence.IPersister persister;
	    private readonly IHost host;
	    private readonly HostSection config;
		private int recursionDepth = 2;
		#endregion

		#region Constructors
		public DynamicSitesProvider(Persistence.IPersister persister, IHost host, HostSection config)
		{
			if (persister == null) throw new ArgumentNullException("persister");
			if (config == null) throw new ArgumentNullException("config");

			this.persister = persister;
		    this.host = host;
		    this.config = config;
		} 
		#endregion

		/// <summary>Gets or sets the number of levels to look for site sources. 0 means root item only. 1 also includes the root item's child pages, etc.</summary>
		public int RecursionDepth
		{
			get { return recursionDepth; }
			set { recursionDepth = value; }
		}


        public virtual IEnumerable<Site> GetSites()
		{
			if (RecursionDepth < 0) throw new N2Exception("The DynamicSitesProvider requires the RecursionDepth property to be at least 0");

            List<Site> foundSites = new List<Site>();

            try
            {
                AddSites(config.RootID, foundSites);
                
                foreach (SiteElement element in config.Sites)
                {
                        if(element.RootID.HasValue)
                        {
                            int rootID = element.RootID.Value;

                            AddSites(rootID, foundSites);
                        }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("DynamicSitesProvider.GetSites:" + ex);
            }

		    return foundSites;
		}

	    private void AddSites(int rootID, List<Site> foundSites)
	    {
	        ContentItem rootItem = persister.Get(rootID);
	        if (rootItem == null)
	            return;

	        foreach (ISitesSource source in new RecursiveFinder().Find<ISitesSource>(rootItem, RecursionDepth))
	        {
	            foreach (Site s in source.GetSites())
	            {
	                foundSites.Add(s);
	            }
	        }
	    }
	}
}
