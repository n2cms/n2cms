using System;
using NHibernate;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Creates and provides access to the NHibernate session in the current 
    /// request's scope.
    /// </summary>
	public interface ISessionProvider: IDisposable
	{
		/// <summary>Tells whether cache should be enabled by default.</summary>
		bool CacheEnabled { get; }

        /// <summary>Returns an already opened session or creates and opens a new one and puts it in the current request.</summary>
        /// <returns>A NHibernate session.</returns>
		SessionContext OpenSession { get; }

		/// <summary>Returns an already opened session or creates and opens a new one and puts it in the current request.</summary>
		/// <returns>A NHibernate session.</returns>
		ISessionFactory SessionFactory { get; }

        /// <summary>Persists changes to disk.</summary>
		void Flush();
	}
}
