using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using N2.Collections;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Web.UI.WebControls
{
    /// <summary>Provides content items to data bound control. It also serves as a hierarchical data source.</summary>
    /// <remarks>This data source has two distinct views. The children view that lists children of a page and the query view that allows database-like queries.</remarks>
    /// <example>
    /// <![CDATA[
    /// List child items of the current item:
    /// <n2:ItemDataSource id="itemSource1" runat="server" />
    /// <asp:Repeater DataSourceID="itemSource1" runat="server">
    ///     <ItemTemplate><p><%# Eval("Title") %></p></ItemTemplate>
    /// </asp:Repeater>
    /// 
    /// List items on the same level as the current item:
    /// <n2:ItemDataSource id="itemSource2" runat="server" Path=".." />
    /// <asp:Repeater DataSourceID="itemSource2" runat="server">
    ///     <ItemTemplate><p><%# Eval("Title") %></p></ItemTemplate>
    /// </asp:Repeater>
    /// 
    /// List items below a child item with the name test (if it exists):
    /// <n2:ItemDataSource id="itemSource3" runat="server" Path="test" ThrowOnInvalidPath="false"/>
    /// <asp:Repeater DataSourceID="itemSource3" runat="server">
    ///     <ItemTemplate><p><%# Eval("Title") %></p></ItemTemplate>
    /// </asp:Repeater>
    /// 
    /// List items with a query:
    /// <n2:ItemDataSource id="itemSource4" runat="server" Query="FROM ContentItem WHERE VersionOfID IS NULL AND Title like ?" QueryParameters='<%$ Code: new object[] {"test%"} %>'/>
    /// <asp:Repeater DataSourceID="itemSource4" DataMember="Query" runat="server">
    ///     <ItemTemplate><p><%# Eval("Title") %></p></ItemTemplate>
    /// </asp:Repeater>
    /// ]]>
    /// </example>
    public class ItemDataSource : DataSourceControl, IItemContainer, IHierarchicalDataSource
    {
        public const string ChildrenViewName = "Children";
        public const string QueryViewName = "Query";

        private ChildrenDataSourceView childrenView = null;

        private ContentItem currentItem = null;
        private IQueryEnding query;
        private QueryDataSourceView queryView = null;

        #region Public Properties

        #region ChildrenView Specific

        /// <summary>A relative path to the item whose children should be listed by this datasource.</summary>
        /// <example>
        /// ItemDataSource1.Path = ".."; // Display other items on the same level.
        /// ItemDataSource1.Path = "lorem"; // Display children of the item with the name 'lorem' below the current item.
        /// ItemDataSource1.Path = "/ipsum"; // Display children of the item with the name 'ipsum' directly below the start page.
        /// ItemDataSource1.Path = "//dolor"; // Display children of the item with the name 'dolor' directly below the root item (this is the same as the start page unless multiple hosts are configured).
        /// ItemDataSource1.Path = "~/"; // Display children of the start page.
        /// </example>
        public virtual string Path
        {
            get { return (string) ViewState["Path"] ?? ""; }
            set
            {
                ViewState["Path"] = value;
                if (currentItem != null)
                {
                    currentItem = null;
                    if (childrenView != null)
                        childrenView.ParentItem = CurrentItem;
                }
            }
        }

        /// <summary>Gets or sets whether an exception should be thrown if the given path is invalid or does not match any item.</summary>
        public bool ThrowOnInvalidPath
        {
            get { return (bool) (ViewState["ThrowOnInvalidPath"] ?? true); }
            set { ViewState["ThrowOnInvalidPath"] = value; }
        }

        public string ZoneName
        {
            get { return (string) ViewState["ZoneName"]; }
            set { ViewState["ZoneName"] = value; }
        }

        public bool PageFilter
        {
            get { return (bool) (ViewState["PageFilter"] ?? false); }
            set { ViewState["PageFilter"] = value; }
        }

        public bool NavigationFilter
        {
            get { return (bool)(ViewState["NavigationFilter"] ?? false); }
            set { ViewState["NavigationFilter"] = value; }
        }

        public string SortBy
        {
            get { return (string)ViewState["SortBy"]; }
            set { ViewState["SortBy"] = value; }
        }

        #endregion

        #region QueryView Specific

        public IQueryEnding Query
        {
            get { return query; }
            set
            {
                query = value;
                GetQueryView().Query = value;
            }
        }

        #endregion

        #endregion

        #region Methods

        protected virtual IEngine Engine
        {
            get { return N2.Context.Current; }
        }

        protected virtual ChildrenDataSourceView GetChildrenView()
        {
            if (childrenView == null)
            {
                childrenView = new ChildrenDataSourceView(Engine, this, ChildrenViewName, CurrentItem);
                List<ItemFilter> filters = new List<ItemFilter>();
                if (ZoneName != null) filters.Add(new ZoneFilter(ZoneName));
                if (PageFilter) filters.Add(new PageFilter());
                if (NavigationFilter) filters.Add(new NavigationFilter());
                childrenView.Filter = new AllFilter(filters);
                childrenView.SortBy = SortBy;
            }
            return childrenView;
        }

        protected virtual QueryDataSourceView GetQueryView()
        {
            if (queryView == null)
            {
                queryView = new QueryDataSourceView(Engine, this, QueryViewName, Query);
            }
            return queryView;
        }

        #endregion

        #region IHierarchicalDataSource Members

        public event EventHandler DataSourceChanged;

        public HierarchicalDataSourceView GetHierarchicalView(string viewPath)
        {
            if (string.IsNullOrEmpty(viewPath))
                return new ItemHierarchicalDataSourceView(CurrentItem);

            return new ItemHierarchicalDataSourceView(ItemUtility.WalkPath(CurrentItem, viewPath));
        }

        #endregion

        #region IItemContainer Members

        public virtual ContentItem CurrentItem
        {
            get
            {
                if (currentItem == null)
                {
                    currentItem = ItemUtility.FindCurrentItem(Parent) ?? N2.Context.CurrentPage;
                    if (!string.IsNullOrEmpty(Path))
                    {
                        currentItem = ItemUtility.WalkPath(currentItem, Path);
                        if (currentItem == null && ThrowOnInvalidPath)
                            throw new InvalidPathException(Path);
                    }
                }
                return currentItem;
            }
            set { currentItem = value; }
        }

        #region Events

        public event EventHandler<ItemListEventArgs> Deleted
        {
            add
            {
                GetChildrenView().Deleted += value;
                GetQueryView().Deleted += value;
            }
            remove
            {
                GetChildrenView().Deleted -= value;
                GetQueryView().Deleted -= value;
            }
        }

        public event EventHandler<ItemListEventArgs> Deleting
        {
            add
            {
                GetChildrenView().Deleting += value;
                GetQueryView().Deleting += value;
            }
            remove
            {
                GetChildrenView().Deleting -= value;
                GetQueryView().Deleting -= value;
            }
        }

        public event EventHandler<ItemListEventArgs> Filtering
        {
            add
            {
                GetChildrenView().Filtering += value;
                GetQueryView().Filtering += value;
            }
            remove
            {
                GetChildrenView().Filtering -= value;
                GetQueryView().Filtering -= value;
            }
        }

        public event EventHandler<ItemListEventArgs> Inserted
        {
            add
            {
                GetChildrenView().Inserted += value;
                GetQueryView().Inserted += value;
            }
            remove
            {
                GetChildrenView().Inserted -= value;
                GetQueryView().Inserted -= value;
            }
        }

        public event EventHandler<ItemListEventArgs> Inserting
        {
            add
            {
                GetChildrenView().Inserting += value;
                GetQueryView().Inserting += value;
            }
            remove
            {
                GetChildrenView().Inserting -= value;
                GetQueryView().Inserting -= value;
            }
        }

        public event EventHandler<ItemEventArgs> ItemCreated
        {
            add { GetChildrenView().ItemCreated += value; }
            remove { GetChildrenView().ItemCreated -= value; }
        }

        public event EventHandler<ItemEventArgs> ItemCreating
        {
            add { GetChildrenView().ItemCreating += value; }
            remove { GetChildrenView().ItemCreating -= value; }
        }

        public event EventHandler<ItemListEventArgs> Selected
        {
            add
            {
                GetChildrenView().Selected += value;
                GetQueryView().Selected += value;
            }
            remove
            {
                GetChildrenView().Selected -= value;
                GetQueryView().Selected -= value;
            }
        }

        public event EventHandler<ItemDataSourceSelectingEventArgs> Selecting
        {
            add
            {
                GetChildrenView().Selecting += value;
                GetQueryView().Selecting += value;
            }
            remove
            {
                GetChildrenView().Selecting -= value;
                GetQueryView().Selecting -= value;
            }
        }

        public event EventHandler<ItemListEventArgs> Updated
        {
            add
            {
                GetChildrenView().Updated += value;
                GetQueryView().Updated += value;
            }
            remove
            {
                GetChildrenView().Updated -= value;
                GetQueryView().Updated -= value;
            }
        }

        public event EventHandler<ItemListEventArgs> Updating
        {
            add
            {
                GetChildrenView().Updating += value;
                GetQueryView().Updating += value;
            }
            remove
            {
                GetChildrenView().Updating -= value;
                GetQueryView().Updating -= value;
            }
        }

        #endregion

        #endregion

        protected override ICollection GetViewNames()
        {
            return new string[] {ChildrenViewName, QueryViewName};
        }

        protected override DataSourceView GetView(string viewName)
        {
            if (viewName == QueryViewName)
                return GetQueryView();
            else
                return GetChildrenView();
        }

        protected override void RaiseDataSourceChangedEvent(EventArgs e)
        {
            base.RaiseDataSourceChangedEvent(e);
            if (DataSourceChanged != null)
                DataSourceChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
