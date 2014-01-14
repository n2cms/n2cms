namespace N2.Persistence.Finder
{
    public interface IOrderBy
    {
        ISortDirection ID { get; }
        ISortDirection Parent { get; }

        ISortDirection Title { get; }
        ISortDirection Name { get; }
        ISortDirection ZoneName { get; }
        ISortDirection Created { get; }
        ISortDirection Updated { get; }
        ISortDirection Published { get; }
        ISortDirection Expires { get; }
        ISortDirection SortOrder { get; }
        ISortDirection VersionIndex { get; }
        ISortDirection State { get; }
        ISortDirection Visible { get; }
        ISortDirection SavedBy { get; }

        ISortDirection Property(string name);
        ISortDirection Detail(string name);
    }
}
