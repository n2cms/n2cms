using System;
using System.Configuration;

namespace NHibernate.Caches.SysCache2
{
    /// <summary>
    /// Configures a sql command notification cache dependency for am NHibernate cache region 
    /// </summary>
    public class CommandCacheDependencyElement : ConfigurationElement
    {
        /// <summary>Holds the configuration property definitions</summary>
        private static readonly ConfigurationPropertyCollection properties;

        /// <summary>
        /// Initializes the <see cref="CacheRegionElement"/> class.
        /// </summary>
        static CommandCacheDependencyElement()
        {
            //building the properties collection and overriding the properties property apparently
            //increases performace considerably
            properties = new ConfigurationPropertyCollection();

            var nameProperty = new ConfigurationProperty("name", typeof (string), String.Empty,
                                                         ConfigurationPropertyOptions.IsKey);

            properties.Add(nameProperty);

            var commandProperty = new ConfigurationProperty("command", typeof (string), String.Empty,
                                                            ConfigurationPropertyOptions.IsRequired);

            properties.Add(commandProperty);

            var commandTimeoutProperty = new ConfigurationProperty("commandTimeout", typeof(int?), null,
                                                            ConfigurationPropertyOptions.None);

            properties.Add(commandTimeoutProperty);

            var connectionNameProperty = new ConfigurationProperty("connectionName", typeof (string), String.Empty,
                                                                   ConfigurationPropertyOptions.None);

            properties.Add(connectionNameProperty);

            var isSprocProperty = new ConfigurationProperty("isStoredProcedure", typeof (bool), false,
                                                            ConfigurationPropertyOptions.None);

            properties.Add(isSprocProperty);

            var providerTypeProperty = new ConfigurationProperty("connectionStringProviderType", typeof (System.Type), null,
                                                                 new TypeNameConverter(),
                                                                 new SubclassTypeValidator(typeof (IConnectionStringProvider)),
                                                                 ConfigurationPropertyOptions.None);

            properties.Add(providerTypeProperty);
        }

        /// <summary>
        /// The unique name of the dependency 
        /// </summary>
        public string Name
        {
            get { return (string) base["name"]; }
        }

        /// <summary>
        /// Gets the sql command statement that will be used to monitor for data changes
        /// </summary>
        [ConfigurationProperty("command", IsRequired = true)]
        public string Command
        {
            get { return (string) base["command"]; }
        }

        /// <summary>
        /// Gets the connection string name for the database
        /// </summary>
        public string ConnectionName
        {
            get { return (string) base["connectionName"]; }
        }

        /// <summary>
        /// Gets whether the <see cref="Command"/> is a stored procedure or not
        /// </summary>
        public bool IsStoredProcedure
        {
            get { return (bool) base["isStoredProcedure"]; }
        }

        /// <summary>
        /// Gets the type of <see cref="IConnectionStringProvider"/> to use when 
        /// retreiving the connection string. 
        /// </summary>
        /// <remarks>
        ///     <para>If no value is supplied, the <see cref="ConfigurationManager"/>
        ///     will be used to retrieve the connection string</para>
        /// </remarks>
        public System.Type ConnectionStringProviderType
        {
            get { return (System.Type) base["connectionStringProviderType"]; }
        }

        /// <summary>
        /// How long the sql command can run for without timing out. If null,
        /// the default is used.
        /// </summary>
        public int? CommandTimeout
        {
            get { return (int?)base["commandTimeout"]; }
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
