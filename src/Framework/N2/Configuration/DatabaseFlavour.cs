namespace N2.Configuration
{
	public enum DatabaseFlavour
	{
		AutoDetect = 0,
		SqlServer = 1,
		SqlServer2008 = 2,
		SqlServer2005 = 3,
		SqlServer2000 = 4,
		SqlCe = 5,
		SqlCe3 = 6,
		SqlCe4 = 7,
		MySql = 8,
		SqLite = 9,
		Firebird = 10,
		Generic = 11,
		Jet = 12,
		DB2 = 13,
		Oracle = 14,
		Oracle9i = 15,
		Oracle10g = 16,
		NoSql = 0xFFF,
		MongoDB = NoSql + 1,
		Xml = NoSql + 2
	}
}
