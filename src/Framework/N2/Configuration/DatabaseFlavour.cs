namespace N2.Configuration
{
    public enum DatabaseFlavour
    {
        AutoDetect = 0,
        SqlServer = 0x1,
        SqlServer2008 = 0x2,
        SqlServer2005 = 0x4,
        SqlServer2000 = 0x8,
        SqlCe = 0x10,
        SqlCe3 = 0x20,
        SqlCe4 = 0x40,
        MySql = 0x80,
        SqLite = 0x100,
        Generic = 0x400,
        Jet = 0x800,
        DB2 = 0x1000,
        Oracle = 0x2000,
        Oracle9i = 0x4000,
        Oracle10g = 0x8000,
        NoSql = 0x10000,
        MongoDB = NoSql | 0x20000,
        Xml = NoSql | 0x40000
    }

}
