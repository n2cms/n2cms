using System.Collections.Generic;
using System.Diagnostics;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Persistence;
using N2.Plugin;

namespace N2.Web
{
	[Service]
	public class MultipleSitesInitializer : IAutoStart
	{
		private readonly Engine.Logger<MultipleSitesInitializer> logger;

		public MultipleSitesInitializer(IPersister persister, IHost host, ISitesProvider sitesProvider, ConnectionMonitor context, HostSection config, IDefinitionManager ignored)
		{
			logger.Debug("MultipleSitesInitializer");

			if (config.MultipleSites && config.DynamicSites)
			{
				context.Online += delegate
				{
					host.AddSites(sitesProvider.GetSites());
					persister.ItemSaved += delegate(object sender, ItemEventArgs e)
					{
						if (e.AffectedItem is ISitesSource)
						{
							IList<Site> sites = Host.ExtractSites(config);
							sites = Host.Union(sites, sitesProvider.GetSites());

							host.ReplaceSites(host.DefaultSite, sites);
						}
					};
				};
			}
		}

		#region IAutoStart Members

		public void Start()
		{
		}

		public void Stop()
		{
		}

		#endregion
	}
}
