using System;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace N2.Edit.Install
{
	public enum ServerType
	{
		SqlServer2005 = 0,
		SqlServer2005Express = 1,
		SqlServer2000 = 2,
		Oracle = 3,
		MySql5 = 4,
		SQLite3 = 5,
		OtherOleDb = 6
	}

	public class InstallationManager
	{
		#region Constructor
		public InstallationManager(ServerType serverType, string connectionString)
		{
			this.serverType = serverType;
			this.connectionString = connectionString;
			string[] resourceKeys = new string[] {
				SqlServer2005ResourceKey,
				SqlServer2005ResourceKey,
				SqlServer2000ResourceKey,
				null,
				MySQLResourceKey,
				SQLiteResourceKey,
				MySQLResourceKey
			};
			resourceKey = resourceKeys[(int)serverType];
		} 
		#endregion

		#region Constants
		public const string SqlServer2005ResourceKey = "N2.Edit.Install.SqlScript.Create.SQLServer2005.sql";
		public const string SqlServer2000ResourceKey = "N2.Edit.Install.SqlScript.Create.SQLServer2000.sql";
		public const string MySQLResourceKey = "N2.Edit.Install.SqlScript.Create.MySQL.sql";
		public const string SQLiteResourceKey = "N2.Edit.Install.SqlScript.Create.SQLite.sql";	
		#endregion

		private string resourceKey;
		private string connectionString;
		private ServerType serverType;

		#region Static Methods
		public static string GetResourceString(string resourcePath)
		{
			System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
			System.IO.StreamReader sr = new System.IO.StreamReader(s);
			return sr.ReadToEnd();
		}
		#endregion
		#region Methods
		public void Install()
		{
			ExecuteSqlResource(resourceKey);
		}
		private void ExecuteCommandWithGoSplitter(string sql)
		{
			using (IDbConnection conn = GetConnection())
			{
				conn.Open();
				IDbCommand cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				int startPos = 0;

				do
				{
					int lastPos = sql.IndexOf("GO", startPos);
					int len = (lastPos > startPos ? lastPos : sql.Length) - startPos;
					string sqlPart = sql.Substring(startPos, len);

					if (sqlPart.Trim().Length > 0)
					{
						cmd.CommandText = sqlPart;
						cmd.ExecuteNonQuery();
					}

					if (lastPos == -1)
						break;
					else
						startPos = lastPos + 2;
				} while (startPos < sql.Length);
			}
		}

		private void ExecuteSqlResource(string resourcePath)
		{
			string sql = GetResourceString(resourcePath);
			ExecuteCommandWithGoSplitter(sql);
		}

		public IDbConnection GetConnection()
		{
			NHibernate.Driver.DriverBase driver;
			switch (serverType)
			{
				case ServerType.SqlServer2005:
				case ServerType.SqlServer2005Express:
				case ServerType.SqlServer2000:
					driver = new NHibernate.Driver.SqlClientDriver();
					break;
				case ServerType.Oracle:
					driver = new NHibernate.Driver.OracleClientDriver();
					break;
				case ServerType.MySql5:
					driver = new NHibernate.Driver.MySqlDataDriver();
					break;
				case ServerType.SQLite3:
					driver = new NHibernate.Driver.SQLite20Driver();
					break;
				default:
					driver = new NHibernate.Driver.OleDbDriver();
					break;
			}
			IDbConnection conn = driver.CreateConnection();
			conn.ConnectionString = connectionString;
			return conn;
		}

		#region GetConnection
		//private static OdbcConnection GetOdbcConnection(string connectionString)
		//{
		//    return new OdbcConnection(connectionString);
		//}

		//private static OleDbConnection GetOleDbConnection(string connectionString)
		//{
		//    return new OleDbConnection(connectionString);
		//}

		//private static SqlConnection GetSqlClientConnection(string connectionString)
		//{
		//    return new SqlConnection(connectionString);
		//}
		#endregion 
		#endregion
	}
}
