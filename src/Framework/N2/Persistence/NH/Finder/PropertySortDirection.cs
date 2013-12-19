namespace N2.Persistence.NH.Finder
{
    public class PropertySortDirection : N2.Persistence.Finder.ISortDirection
    {
        public string sortProperty;
        QueryBuilder query;

        public PropertySortDirection(string sortProperty, QueryBuilder query)
        {
            this.sortProperty = sortProperty;
            this.query = query;
        }

        #region ISortDirection Members

        public N2.Persistence.Finder.IQueryEnding Asc
        {
            get 
            {
                query.OrderBy = this.sortProperty; 
                return query; 
            }
        }

        public N2.Persistence.Finder.IQueryEnding Desc
        {
            get
            {
                query.OrderBy = this.sortProperty + " desc"; 
                return query;
            }
        }

        #endregion
    }
}
