using System.Configuration;

namespace NHibernate.Caches.SysCache2
{
    /// <summary>
    /// Defines the cache dependencies for an NHibernate cache region
    /// </summary>
    public class CacheDependenciesElement : ConfigurationElement
    {
        /// <summary>Holds the configuration property definitions</summary>
        private static readonly ConfigurationPropertyCollection properties;

        /// <summary>
        /// Initializes the <see cref="CacheRegionElement"/> class.
        /// </summary>
        static CacheDependenciesElement()
        {
            //building the properties collection and overriding the properties property apparently
            //increases performace considerably
            properties = new ConfigurationPropertyCollection();

            var tablesProperty = new ConfigurationProperty("tables", typeof (TableCacheDependencyCollection), null,
                                                           ConfigurationPropertyOptions.None);

            properties.Add(tablesProperty);

            var commandsProperty = new ConfigurationProperty("commands", typeof (CommandCacheDependencyCollection), null,
                                                             ConfigurationPropertyOptions.None);

            properties.Add(commandsProperty);
        }

        /// <summary>
        /// Gets the collection of <see cref="TableCacheDependencyElement"/> objects stored within this section.
        /// </summary>
        public TableCacheDependencyCollection TableDependencies
        {
            get { return (TableCacheDependencyCollection) base["tables"]; }
        }

        /// <summary>
        /// Gets the collection of <see cref="CommandCacheDependencyElement"/> objects stored within this section.
        /// </summary>
        public CommandCacheDependencyCollection CommandDependencies
        {
            get { return (CommandCacheDependencyCollection) base["commands"]; }
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
