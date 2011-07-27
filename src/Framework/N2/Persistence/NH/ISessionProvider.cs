using System;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Creates and provides access to the NHibernate session in the current 
    /// request's scope.
    /// </summary>
	public interface ISessionProvider: IDisposable
	{
        /// <summary>Returns an already opened session or creates and opens a new one and puts it in the current request.</summary>
        /// <returns>A NHibernate session.</returns>
        SessionContext OpenSession { get; }

        /// <summary>Returns a new session and puts it in the current request.</summary>
        /// <returns>A NHibernate session.</returns>
        SessionContext CreateSession { get; }

        /// <summary>Persists changes to disk.</summary>
		void Flush();
	}
}
