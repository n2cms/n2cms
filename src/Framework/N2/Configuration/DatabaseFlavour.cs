namespace N2.Configuration
{
	public enum DatabaseFlavour
	{
		AutoDetect = 0,
		SqlServer,
		SqlServer2008,
		SqlServer2005,
		SqlServer2000,
		SqlCe,
		SqlCe3,
		SqlCe4,
		MySql,
		SqLite,
		Firebird,
		Generic,
		Jet,
		DB2,
		Oracle,
		Oracle9i,
		Oracle10g,
		NoSql = 1024,
		MongoDB = NoSql + 1,
		Xml
	}
}
