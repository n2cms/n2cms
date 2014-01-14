namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Transition to the property to order by.
    /// </summary>
    public class OrderBy : N2.Persistence.Finder.IOrderBy
    {
        private QueryBuilder query;

        public OrderBy(QueryBuilder query)
        {
            this.query = query;
        }

        #region IOrderBy Members

        public N2.Persistence.Finder.ISortDirection ID
        {
            get { return new PropertySortDirection("ID", query); }
        }

        public N2.Persistence.Finder.ISortDirection Parent
        {
            get { return new PropertySortDirection("Parent", query); }
        }

        public N2.Persistence.Finder.ISortDirection Title
        {
            get { return new PropertySortDirection("Title", query); }
        }

        public N2.Persistence.Finder.ISortDirection Name
        {
            get { return new PropertySortDirection("Name", query); }
        }

        public N2.Persistence.Finder.ISortDirection ZoneName
        {
            get { return new PropertySortDirection("ZoneName", query); }
        }

        public N2.Persistence.Finder.ISortDirection Created
        {
            get { return new PropertySortDirection("Created", query); }
        }

        public N2.Persistence.Finder.ISortDirection Updated
        {
            get { return new PropertySortDirection("Updated", query); }
        }

        public N2.Persistence.Finder.ISortDirection Published
        {
            get { return new PropertySortDirection("Published", query); }
        }

        public N2.Persistence.Finder.ISortDirection Expires
        {
            get { return new PropertySortDirection("Expires", query); }
        }

        public N2.Persistence.Finder.ISortDirection SortOrder
        {
            get { return new PropertySortDirection("SortOrder", query); }
        }

        public N2.Persistence.Finder.ISortDirection VersionIndex
        {
            get { return new PropertySortDirection("VersionIndex", query); }
        }

        public N2.Persistence.Finder.ISortDirection State
        {
            get { return new PropertySortDirection("State", query); }
        }

        public N2.Persistence.Finder.ISortDirection Visible
        {
            get { return new PropertySortDirection("Visible", query); }
        }

        public N2.Persistence.Finder.ISortDirection SavedBy
        {
            get { return new PropertySortDirection("SavedBy", query); }
        }

        public N2.Persistence.Finder.ISortDirection Property(string name)
        {
            return new PropertySortDirection(name, query);
        }

        public N2.Persistence.Finder.ISortDirection Detail(string name)
        {
            return new DetailSortDirection(name, query);
        }

        #endregion
    }
}
