namespace N2.Persistence.NH.Finder
{
    public class DetailSortDirection : N2.Persistence.Finder.ISortDirection
    {
        public string sortProperty;
        QueryBuilder query;

        public DetailSortDirection(string sortProperty, QueryBuilder query)
        {
            this.sortProperty = sortProperty;
            this.query = query;
        }

        #region ISortDirection Members

        public N2.Persistence.Finder.IQueryEnding Asc
        {
            get 
            {
                query.SortExpression = this.sortProperty; 
                return query; 
            }
        }

        public N2.Persistence.Finder.IQueryEnding Desc
        {
            get
            {
                query.SortExpression = this.sortProperty + " DESC"; 
                return query;
            }
        }

        #endregion
    }
}
