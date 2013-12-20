using System;
using System.Configuration;
using System.Web.Caching;

namespace NHibernate.Caches.SysCache2
{
    /// <summary>
    /// Represents a cacheRegion configuration element
    /// </summary>
    public class CacheRegionElement : ConfigurationElement
    {
        /// <summary>Holds the configuration property definitions</summary>
        private static readonly ConfigurationPropertyCollection properties;

        /// <summary>
        /// Initializes the <see cref="CacheRegionElement"/> class.
        /// </summary>
        static CacheRegionElement()
        {
            //building the properties collection and overriding the properties property apparently
            //increases performace considerably
            properties = new ConfigurationPropertyCollection();

            var nameProperty = new ConfigurationProperty("name", typeof (string), String.Empty,
                                                         ConfigurationPropertyOptions.IsKey);

            properties.Add(nameProperty);

            var relativeExpirationProperty = new ConfigurationProperty("relativeExpiration", typeof (TimeSpan?), null,
                                                                       new TimeSpanSecondsConverter(), null,
                                                                       ConfigurationPropertyOptions.None);

            properties.Add(relativeExpirationProperty);

            var timeOfDayExpirationProperty = new ConfigurationProperty("timeOfDayExpiration", typeof (TimeSpan?), null, null,
                                                                        new NullableTimeSpanValidator(new TimeSpan(0, 0, 0),
                                                                                                      new TimeSpan(23, 59, 59),
                                                                                                      false),
                                                                        ConfigurationPropertyOptions.None);

            properties.Add(timeOfDayExpirationProperty);

            var priorityProperty = new ConfigurationProperty("priority", typeof (CacheItemPriority), CacheItemPriority.Default,
                                                             ConfigurationPropertyOptions.None);

            properties.Add(priorityProperty);

            var dependenciesProperty = new ConfigurationProperty("dependencies", typeof (CacheDependenciesElement), null,
                                                                 ConfigurationPropertyOptions.None);

            properties.Add(dependenciesProperty);
        }

        /// <summary>
        /// The name of the region that the objects to cache will be stored in
        /// </summary>
        public string Name
        {
            get { return (string) base["name"]; }
        }

        /// <summary>
        /// The number of seconds from the time an object is added into the cache until it will 
        /// expire from the cache.
        /// </summary>
        public TimeSpan? RelativeExpiration
        {
            get { return (TimeSpan?) base["relativeExpiration"]; }
        }

        /// <summary>
        /// The time of day in 24 hour format that an object added into the cache will 
        /// expire from the cache.
        /// </summary>
        /// <remarks>
        ///     <para>Must be entered in TimeSpan format. Ex. 13:52:00 would be 1:52 pm</para>
        /// </remarks>
        public TimeSpan? TimeOfDayExpiration
        {
            get { return (TimeSpan?) base["timeOfDayExpiration"]; }
        }

        /// <summary>
        /// Specifies the relative priority of items stored in the Cache region 
        /// </summary>
        public CacheItemPriority Priority
        {
            get { return (CacheItemPriority) base["priority"]; }
        }

        /// <summary>
        /// Gets the depencies configured within this section.
        /// </summary>
        public CacheDependenciesElement Dependencies
        {
            get { return (CacheDependenciesElement) base["dependencies"]; }
        }

        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"></see> collection of properties for the element.</returns>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }
    }
}
