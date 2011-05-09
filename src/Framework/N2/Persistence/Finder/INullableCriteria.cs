namespace N2.Persistence.Finder
{
    public interface INullableCriteria
    {
        IQueryAction IsNull();

        IQueryAction IsNotNull();
    }

    public interface INullableCriteria<T> : ICriteria<T>
    {
        IQueryAction IsNull();

        IQueryAction IsNotNull();
    }
}
