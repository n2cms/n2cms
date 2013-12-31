using System;
using System.Collections.Generic;
using NHibernate.Cache;

namespace NHibernate.Caches.SysCache2
{
    /// <summary>
    /// Cache provider using the System.Web.Caching classes 
    /// </summary>
    public class SysCacheProvider : ICacheProvider
    {
        /// <summary>pre configured cache region settings</summary>
        private static readonly Dictionary<string, SysCacheRegion> cacheRegions;

        /// <summary>list of pre configured already built cache regions</summary>
        private static readonly CacheRegionCollection cacheRegionSettingsList;

        /// <summary>log4net logger</summary>
        private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(SysCacheProvider));

        /// <summary>synchronizing object for the cache regions dictionaly</summary>
        private static readonly Object regionsSyncRoot = new Object();

        /// <summary>
        /// Initializes the <see cref="SysCacheProvider"/> class.
        /// </summary>
        static SysCacheProvider()
        {
            //we need to determine which cache regions are configured in the configuration file, but we cant create the 
            //cache regions at this time becasue there could be nhibernate configuration values
            //that we need for the cache regions such as connection info to be used for data dependencies, but this info 
            //isnt available until until build cache is called.  So allocte space but only create them on demand

            SysCacheSection configSection = SysCacheSection.GetSection();

            if (configSection != null && configSection.CacheRegions.Count > 0)
            {
                cacheRegionSettingsList = configSection.CacheRegions;
                cacheRegions = new Dictionary<string, SysCacheRegion>(cacheRegionSettingsList.Count);
            }
            else
            {
                cacheRegions = new Dictionary<string, SysCacheRegion>(0);
                log.Info(
                    "No cache regions specified. Cache regions can be specified in sysCache configuration section with custom settings.");
            }
        }

        #region ICacheProvider Members

        /// <summary>
        /// Configure the cache
        /// </summary>
        /// <param name="regionName">the name of the cache region</param>
        /// <param name="properties">configuration settings</param>
        /// <returns></returns>
        public ICache BuildCache(string regionName, IDictionary<string, string> properties)
        {
            //return a configured cache region if we have one for the region already
            //the only way this will really happen is if there is a query cache specified for a region that is configured
            //since query caches are not configured at session factory startup 
            if (String.IsNullOrEmpty(regionName) == false && cacheRegions.ContainsKey(regionName))
            {
                return cacheRegions[regionName];
            }

            //build the cache from preconfigured values if the region has configuration values
            if (cacheRegionSettingsList != null)
            {
                CacheRegionElement regionSettings = cacheRegionSettingsList[regionName];

                if (regionSettings != null)
                {
                    SysCacheRegion cacheRegion;

                    lock (regionsSyncRoot)
                    {
                        //note that the only reason we have to do this double check is because the query cache 
                        //can try to create caches at unpredictable times
                        if (cacheRegions.TryGetValue(regionName, out cacheRegion) == false)
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("building cache region, '{0}', from configuration", regionName);
                            }

                            //build the cache region with settings and put it into the list so that this proces will not occur again
                            cacheRegion = new SysCacheRegion(regionName, regionSettings, properties);
                            cacheRegions[regionName] = cacheRegion;
                        }
                    }

                    return cacheRegion;
                }
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("building non-configured cache region : {0}", regionName);
            }

            //we will end up creating cache regions here for cache regions that nhibernate
            //uses internally and cache regions that weren't specified in the application config file
            return new SysCacheRegion(regionName, properties);
        }

        /// <summary>
        /// Generate a timestamp
        /// </summary>
        /// <returns>A Timestamp</returns>
        public long NextTimestamp()
        {
            return Timestamper.Next();
        }

        /// <summary>
        /// Callback to perform any necessary initialization of the underlying cache implementation
        /// during ISessionFactory construction.
        /// </summary>
        /// <param name="properties">current configuration settings</param>
        public void Start(IDictionary<string, string> properties) {}

        /// <summary>
        /// Callback to perform any necessary cleanup of the underlying cache implementation
        /// during <see cref="M:NHibernate.ISessionFactory.Close"/>.
        /// </summary>
        public void Stop() {}

        #endregion
    }
}
