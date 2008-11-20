namespace N2.Engine
{
    public delegate ReturnT Function<ReturnT>();
	public delegate ReturnT Function<T1, ReturnT>(T1 argument);
	public delegate ReturnT Function<T1, T2, ReturnT>(T1 arg1, T2 arg2);
}
