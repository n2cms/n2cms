using System;
using System.Web.Caching;

namespace NHibernate.Caches.SysCache2
{
    /// <summary>
    /// Creates SqlCacheDependency objects dependent on data changes in a table and registers the dependency for 
    /// change notifications if necessary
    /// </summary>
    public class SqlTableCacheDependencyEnlister : ICacheDependencyEnlister
    {
        /// <summary>the name of the database entry to use for connection info</summary>
        private readonly string databaseEntryName;

        /// <summary>the name of the table to monitor</summary>
        private readonly string tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTableCacheDependencyEnlister"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table to monitor</param>
        /// <param name="databaseEntryName">The name of the database entry to use for connection information</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tableName"/> or 
        ///     <paramref name="databaseEntryName"/> is null or empty.</exception>
        public SqlTableCacheDependencyEnlister(string tableName, string databaseEntryName)
        {
            //validate the params
            if (String.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (String.IsNullOrEmpty(databaseEntryName))
            {
                throw new ArgumentNullException("databaseEntryName");
            }

            this.tableName = tableName;
            this.databaseEntryName = databaseEntryName;
        }

        #region ICacheDependencyEnlister Members

        /// <summary>
        /// Enlists a cache dependency to recieve change notifciations with an underlying resource
        /// </summary>
        /// <returns>
        /// The cache dependency linked to the notification subscription
        /// </returns>
        public CacheDependency Enlist()
        {
            //there is no need to enlist in notification subscription for this type of dependency

            return new SqlCacheDependency(databaseEntryName, tableName);
        }

        #endregion
    }
}
