using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;

namespace NHibernate.Caches.SysCache2
{
	/// <summary>
	/// Creates SqlCacheDependency objects and hooks them up to a query notification based on the command
	/// </summary>
	public class SqlCommandCacheDependencyEnlister : ICacheDependencyEnlister
	{
		/// <summary>sql command to use for creating notifications</summary>
		private readonly string command;

        /// <summary>SQL command timeout. If null, the default is used.</summary>
        private readonly int? commandTimeout;

		/// <summary>The name of the connection string</summary>
		private readonly string connectionName;

		/// <summary>The connection string to use for connection to the date source</summary>
		private readonly string connectionString;

		/// <summary>indicates if the command is a stored procedure or not</summary>
		private readonly bool isStoredProcedure;

	    /// <summary>
		/// Initializes a new instance of the <see cref="SqlCommandCacheDependencyEnlister"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
		/// <param name="connectionStringProvider">The <see cref="IConnectionStringProvider"/> to use 
		///		to retrieve the connection string to connect to the underlying data store and enlist in query notifications</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="command"/> or 
		///		<paramref name="connectionStringProvider"/> is null or empty.</exception>
		public SqlCommandCacheDependencyEnlister(string command, bool isStoredProcedure,
		                                         IConnectionStringProvider connectionStringProvider)
			: this(command, isStoredProcedure, null, null, connectionStringProvider) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCommandCacheDependencyEnlister"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
        /// <param name="commandTimeout">The command timeout in seconds. If null, the default is used.</param>
		/// <param name="connectionName">Name of the connection.</param>
		/// <param name="connectionStringProvider">The <see cref="IConnectionStringProvider"/> to use 
		///		to retrieve the connection string to connect to the underlying data store and enlist in query notifications</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="command"/> or 
		///		<paramref name="connectionStringProvider"/> is null or empty.</exception>
        public SqlCommandCacheDependencyEnlister(string command, bool isStoredProcedure, 
            int? commandTimeout, string connectionName, IConnectionStringProvider connectionStringProvider)
		{
			//validate the parameters
			if (String.IsNullOrEmpty(command))
			{
				throw new ArgumentNullException("command");
			}

			if (connectionStringProvider == null)
			{
				throw new ArgumentNullException("connectionStringProvider");
			}

			this.command = command;
			this.isStoredProcedure = isStoredProcedure;
            this.commandTimeout = commandTimeout;
		    this.connectionName = connectionName;

			connectionString = String.IsNullOrEmpty(this.connectionName) ? connectionStringProvider.GetConnectionString() : connectionStringProvider.GetConnectionString(this.connectionName);
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
			SqlCacheDependency dependency;

			//setup and execute the command that will register the cache dependency for
			//change notifications
			using (var connection = new SqlConnection(connectionString))
			{
				using (var exeCommand = new System.Data.SqlClient.SqlCommand(command, connection))
				{
					//is the command a sproc
					if (isStoredProcedure)
					{
						exeCommand.CommandType = CommandType.StoredProcedure;
					}

                    if (commandTimeout.HasValue)
				        exeCommand.CommandTimeout = this.commandTimeout.Value;

					//hook the deondency up to the command
					dependency = new SqlCacheDependency(exeCommand);

					connection.Open();
					//execute the query, this will enlist the dependency. Notice that we execute a non query since
					//we dont need any results
					exeCommand.ExecuteNonQuery();
				}
			}

			return dependency;
		}

		#endregion
	}
}