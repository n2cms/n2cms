using System.Collections;
using System.Collections.Generic;
using System.Web.UI;

namespace N2.Web.UI
{
    /// <summary>A DataSourceView that can perform queries against the N2 persistence manager.</summary>
    public class QueryDataSourceView : ItemDataSourceView
    {
        #region Constructors
        public QueryDataSourceView(Engine.IEngine engine, IDataSource owner, string viewName, N2.Persistence.Finder.IQueryEnding query)
            : base(engine, owner, viewName)
        {
            this.query = query;
        }
        #endregion

        #region Private Members
        private N2.Persistence.Finder.IQueryEnding query;
        #endregion

        #region Properties
        public N2.Persistence.Finder.IQueryEnding Query
        {
            get { return query; }
            set { query = value; }
        }

        #endregion

        #region Methods
        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            if (this.Query != null)
            {
                ItemDataSourceSelectingEventArgs selectingArgs = new ItemDataSourceSelectingEventArgs(arguments);
                OnSelecting(selectingArgs);

                IList<ContentItem> items = Query
                    .FirstResult(selectingArgs.Arguments.StartRowIndex)
                    .MaxResults(selectingArgs.Arguments.MaximumRows)
                    .Select();

                Collections.ItemListEventArgs args = new N2.Collections.ItemListEventArgs(new Collections.ItemList(items));
                OnSelected(args);
                return args.Items;
            }
            else
                return null;
        }
        #endregion
    }
}
