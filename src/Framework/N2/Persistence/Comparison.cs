namespace N2.Persistence
{
    public enum Comparison
    {
        Equal,
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual,
        Like,
        Null,
        In,
        Not = 1024,
        NotEqual = Not | Equal,
        NotLike = Not | Like,
        NotNull = Not | Null,
        NotIn = Not | In
    }
}
