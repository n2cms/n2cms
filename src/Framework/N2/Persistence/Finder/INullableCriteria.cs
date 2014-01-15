namespace N2.Persistence.Finder
{
    public interface INullableCriteria
    {
        IQueryAction IsNull();

        IQueryAction IsNotNull();
    }
}
