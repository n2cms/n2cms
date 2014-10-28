using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using N2.Definitions;
using N2.Persistence;
using N2.Security;

namespace N2.Web.UI
{
    /// <summary>A data source view that provides the child items of a parent item.</summary>
    public class ChildrenDataSourceView : ItemDataSourceView
    {
        #region Private Fields
        private ContentItem parentItem = null;
        private Collections.ItemList allItems = null;
        private Collections.AllFilter filter;
        #endregion

        #region Constructors
        public ChildrenDataSourceView(Engine.IEngine engine, IDataSource owner, string viewName, ContentItem parentItem)
            : base(engine, owner, viewName)
        {
            this.parentItem = parentItem;
        }       
        #endregion

        #region Properties
        public ContentItem ParentItem
        {
            get { return parentItem; }
            set
            {
                parentItem = value;
                this.allItems = null;
                this.OnDataSourceViewChanged(EventArgs.Empty);
            }
        }

        public Collections.AllFilter Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        [Obsolete("Use Filter instaed")]
        public IEnumerable<Collections.ItemFilter> Filters
        {
            set { filter = new Collections.AllFilter(value); }
        }

        public string SortBy { get; set; }

        public override bool CanInsert
        {
            get { return parentItem != null; }
        }
        public override bool CanRetrieveTotalRowCount
        {
            get { return true; }
        }
        public override bool CanSort
        {
            get { return true; }
        }
        #endregion

        #region Methods
        #region On...
        protected virtual void OnItemCreated(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = base.Events[EventItemCreated] as EventHandler<ItemEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        protected virtual void OnItemCreating(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = base.Events[EventItemCreating] as EventHandler<ItemEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);

            if (e.AffectedItem != null)
            {
                IDefinitionManager definitions = Engine.Definitions;
                ISecurityManager security = Engine.SecurityManager;
                ContentActivator activator = Engine.Resolve<ContentActivator>();
                ItemDefinition parentDefinition = definitions.GetDefinition(parentItem);

                if (parentDefinition.IsChildAllowed(definitions, parentItem, parentDefinition))
                {
                    e.AffectedItem = Engine.Resolve<ContentActivator>().CreateInstance(parentItem.GetContentType(), parentItem);
                    return;
                }
                foreach (ItemDefinition definition in definitions.GetAllowedChildren(parentItem, null).WhereAuthorized(security, HttpContext.Current.User, parentItem))
                {
                    e.AffectedItem = activator.CreateInstance(definition.ItemType, parentItem);
                    return;
                }
                throw new N2.Definitions.NoItemAllowedException(parentItem);
            }
        }
        #endregion

        private Collections.ItemList GetAllItems()
        {
			if (allItems == null && parentItem != null)
				allItems = parentItem.Children.WhereAccessible();
            return allItems;
        }

        private string currentSortExpression = "";
        protected override System.Collections.IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            ItemDataSourceSelectingEventArgs selectingArgs = new ItemDataSourceSelectingEventArgs(arguments);
            OnSelecting(selectingArgs);
            Collections.ItemListEventArgs args = new N2.Collections.ItemListEventArgs(GetAllItems());
            if (args.Items == null)
                return null;
            
            if(selectingArgs.Arguments.RetrieveTotalRowCount)
                selectingArgs.Arguments.TotalRowCount = args.Items.Count;

            if (!string.IsNullOrEmpty(selectingArgs.Arguments.SortExpression) && selectingArgs.Arguments.SortExpression != currentSortExpression)
            {
                args.Items.Sort(new Collections.ItemComparer(selectingArgs.Arguments.SortExpression));
                currentSortExpression = selectingArgs.Arguments.SortExpression;
            }
            else if (!string.IsNullOrEmpty(SortBy))
            {
                args.Items.Sort(new Collections.ItemComparer(SortBy));
            }

            OnFiltering(args);
            if (Filter != null || (selectingArgs.Arguments.StartRowIndex >= 0 && selectingArgs.Arguments.MaximumRows > 0))
            {
                Collections.ItemList filteredItems = args.Items;
                
                if (Filter != null)
                    filteredItems = new Collections.ItemList(filteredItems, Filter);

                if (selectingArgs.Arguments.StartRowIndex >= 0 && selectingArgs.Arguments.MaximumRows > 0)
                    filteredItems = new Collections.ItemList(filteredItems, new Collections.CountFilter(selectingArgs.Arguments.StartRowIndex, selectingArgs.Arguments.MaximumRows));
                
                args = new N2.Collections.ItemListEventArgs(filteredItems);
            }

            OnSelected(args);
            return args.Items;
        }

        protected override int ExecuteInsert(System.Collections.IDictionary values)
        {
            if (parentItem == null)
                throw new ArgumentNullException("parentItem", "Can't insert item since we have no parent item to insert below");

            ItemEventArgs args = new ItemEventArgs(null);
            OnItemCreating(args);
            OnItemCreated(args);

            if (args.AffectedItem != null)
            {
                // Insert new values
                args.AffectedItem.Parent = parentItem;
                foreach (string propertyName in values.Keys)
                    args.AffectedItem[propertyName] = values[propertyName];

                OnInserting(args);
                Engine.Persister.Save(args.AffectedItem);
                GetAllItems().Add(args.AffectedItem);
                OnInserted(args);
                OnDataSourceViewChanged(EventArgs.Empty);

                return 1;
            }
            return 0;
        }
 
        #endregion  

        #region Events
        public event EventHandler<ItemEventArgs> ItemCreated
        {
            add { base.Events.AddHandler(EventItemCreated, value); }
            remove { base.Events.RemoveHandler(EventItemCreated, value); }
        }
        public event EventHandler<ItemEventArgs> ItemCreating
        {
            add { base.Events.AddHandler(EventItemCreating, value); }
            remove { base.Events.RemoveHandler(EventItemCreating, value); }
        }
        #endregion

        #region Static Event Keys
        private static readonly object EventItemCreated = new object();
        private static readonly object EventItemCreating = new object();
        #endregion  
    }
}
