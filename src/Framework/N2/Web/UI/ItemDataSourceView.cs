using System;
using System.Web.UI;

namespace N2.Web.UI
{
    /// <summary>An abstract base class for item data source views.</summary>
    public abstract class ItemDataSourceView : DataSourceView
    {
        private Engine.IEngine engine;

        protected virtual Engine.IEngine Engine
        {
            get { return engine; }
        }

        #region Constructors
        public ItemDataSourceView(Engine.IEngine engine, IDataSource owner, string viewName)
            : base(owner, viewName)
        {
            this.engine = engine;
        }
        #endregion

        #region Properties  
        public override bool CanDelete
        {
            get { return true; }
        }
        public override bool CanUpdate
        {
            get { return true; }
        }
        public override bool CanPage
        {
            get { return true; }
        }
        #endregion

        #region Methods
        #region On...
        protected virtual void OnDeleted(Collections.ItemListEventArgs e)
        {
            EventHandler<Collections.ItemListEventArgs> handler = base.Events[EventDeleted] as EventHandler<Collections.ItemListEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        protected virtual void OnDeleting(Collections.ItemListEventArgs e)
        {
            EventHandler<Collections.ItemListEventArgs> handler = base.Events[EventDeleting] as EventHandler<Collections.ItemListEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }

        protected virtual void OnFiltering(Collections.ItemListEventArgs e)
        {
            EventHandler<Collections.ItemListEventArgs> handler = base.Events[EventFiltering] as EventHandler<Collections.ItemListEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        
        protected virtual void OnInserted(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = base.Events[EventInserted] as EventHandler<ItemEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        protected virtual void OnInserting(ItemEventArgs e)
        {
            EventHandler<ItemEventArgs> handler = base.Events[EventInserting] as EventHandler<ItemEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        
        protected virtual void OnSelecting(ItemDataSourceSelectingEventArgs e)
        {
            EventHandler<ItemDataSourceSelectingEventArgs> handler = base.Events[EventSelecting] as EventHandler<ItemDataSourceSelectingEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        protected virtual void OnSelected(Collections.ItemListEventArgs e)
        {
            EventHandler<Collections.ItemListEventArgs> handler = base.Events[EventSelected] as EventHandler<Collections.ItemListEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }

        protected virtual void OnUpdated(Collections.ItemListEventArgs e)
        {
            EventHandler<Collections.ItemListEventArgs> handler = base.Events[EventUpdated] as EventHandler<Collections.ItemListEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }
        protected virtual void OnUpdating(Collections.ItemListEventArgs e)
        {
            EventHandler<Collections.ItemListEventArgs> handler = base.Events[EventUpdating] as EventHandler<Collections.ItemListEventArgs>;
            if (handler != null)
                handler.Invoke(this, e);
        }

        #endregion

        #region Delete & Update
        protected virtual Collections.ItemList GetItemsFromIdentifiers(System.Collections.IDictionary keys)
        {
            Collections.ItemList items = new N2.Collections.ItemList();
            foreach (int itemID in keys.Values)
                items.Add(Engine.Persister.Get(itemID));
            return items;
        }

        protected override int ExecuteDelete(System.Collections.IDictionary keys, System.Collections.IDictionary oldValues)
        {
            Collections.ItemListEventArgs args = new N2.Collections.ItemListEventArgs(GetItemsFromIdentifiers(keys));
            OnDeleting(args);

            foreach(ContentItem item in args.Items)
                Engine.Persister.Delete(item);

            OnDeleted(args);

            if (args.Items.Count > 0)
                OnDataSourceViewChanged(EventArgs.Empty);
            return args.Items.Count;
        }

        protected override int ExecuteUpdate(System.Collections.IDictionary keys, System.Collections.IDictionary values, System.Collections.IDictionary oldValues)
        {
            Collections.ItemListEventArgs args = new N2.Collections.ItemListEventArgs(GetItemsFromIdentifiers(keys));
            OnUpdating(args);

            foreach (ContentItem item in args.Items)
            {
                foreach (string propertyName in values.Keys)
                    item[propertyName] = values[propertyName];
                Engine.Persister.Save(item);
            }

            OnUpdated(args);
            if (args.Items.Count > 0)
                OnDataSourceViewChanged(EventArgs.Empty);
            return args.Items.Count;
        } 
        #endregion

        #endregion

        #region Events
        public event EventHandler<Collections.ItemListEventArgs> Deleted
        {
            add { base.Events.AddHandler(EventDeleted, value); }
            remove { base.Events.RemoveHandler(EventDeleted, value); }
        }
        public event EventHandler<Collections.ItemListEventArgs> Deleting
        {
            add { base.Events.AddHandler(EventDeleting, value); }
            remove { base.Events.RemoveHandler(EventDeleting, value); }
        }
        public event EventHandler<Collections.ItemListEventArgs> Filtering
        {
            add { base.Events.AddHandler(EventFiltering, value); }
            remove { base.Events.RemoveHandler(EventFiltering, value); }
        }
        public event EventHandler<Collections.ItemListEventArgs> Inserted
        {
            add { base.Events.AddHandler(EventInserted, value); }
            remove { base.Events.RemoveHandler(EventInserted, value); }
        }
        public event EventHandler<Collections.ItemListEventArgs> Inserting
        {
            add { base.Events.AddHandler(EventInserting, value); }
            remove { base.Events.RemoveHandler(EventInserting, value); }
        }
        public event EventHandler<Collections.ItemListEventArgs> Selected
        {
            add { base.Events.AddHandler(EventSelected, value); }
            remove { base.Events.RemoveHandler(EventSelected, value); }
        }
        public event EventHandler<ItemDataSourceSelectingEventArgs> Selecting
        {
            add { base.Events.AddHandler(EventSelecting, value); }
            remove { base.Events.RemoveHandler(EventSelecting, value); }
        }

        public event EventHandler<Collections.ItemListEventArgs> Updated
        {
            add { base.Events.AddHandler(EventUpdated, value); }
            remove { base.Events.RemoveHandler(EventUpdated, value); }
        }
        public event EventHandler<Collections.ItemListEventArgs> Updating
        {
            add { base.Events.AddHandler(EventUpdating, value); }
            remove { base.Events.RemoveHandler(EventUpdating, value); }
        }
        #endregion

        #region Static Event Keys
        private static readonly object EventDeleted = new object();
        private static readonly object EventDeleting = new object();
        private static readonly object EventFiltering = new object();
        private static readonly object EventInserted = new object();
        private static readonly object EventInserting = new object();
        //private static object EventObjectDisposing = new object();
        private static readonly object EventSelected = new object();
        private static readonly object EventSelecting = new object();
        private static readonly object EventUpdated = new object();
        private static readonly object EventUpdating = new object();


        #endregion
    }
}
