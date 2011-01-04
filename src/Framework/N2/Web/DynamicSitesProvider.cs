using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using N2.Collections;
using N2.Configuration;
using N2.Engine;
using N2.Definitions;
using N2.Persistence;
using N2.Persistence.Finder;

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
		readonly IPersister persister;
		readonly IDefinitionManager definitions;
		readonly IItemFinder finder;
	    readonly IHost host;
	    readonly HostSection config;
		#endregion

		#region Constructors
		public DynamicSitesProvider(Persistence.IPersister persister, IItemFinder finder, IDefinitionManager definitions, IHost host, HostSection config)
		{
			this.persister = persister;
			this.finder = finder;
			this.definitions = definitions;
		    this.host = host;
		    this.config = config;
		} 
		#endregion

        public virtual IEnumerable<Site> GetSites()
		{
            List<Site> foundSites = new List<Site>();

            try
            {
				foreach (ItemDefinition definition in definitions.GetDefinitions())
				{
					if (typeof(ISitesSource).IsAssignableFrom(definition.ItemType))
					{
						foreach (ISitesSource source in finder.Where.Type.Eq(definition.ItemType).Select())
						{
							foreach (Site s in source.GetSites())
							{
								foundSites.Add(s);
							}
						}
					}
				}
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("DynamicSitesProvider.GetSites:" + ex);
            }

		    return foundSites;
		}
	}
}
