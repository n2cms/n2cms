namespace N2.Web.UI
{
    /// <summary>A user control that can be dynamically created, bound to non-page items and added to a page.</summary>
    /// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
    /// <typeparam name="TPart">The type of non-page (data) item this user control will be bound to.</typeparam>
    public abstract class ContentUserControl<TPage, TPart> : ContentUserControl<TPage>, IItemContainer, IContentTemplate
        where TPage : N2.ContentItem
        where TPart : N2.ContentItem
    {
        private TPart currentItem = null;

        /// <summary>Gets the current data item of the dynamically added part.</summary>
        public new TPart CurrentItem
        {
            get { return this.currentItem ?? (currentItem = ItemUtility.CurrentContentItem as TPart); }
            set { currentItem = value; }
        }

        /// <summary>Gets whether the current data item is referenced in the query string. This usually occurs when the item is selected in edit mode.</summary>
        public bool IsHighlighted
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString[PathData.ItemQueryKey]))
                    return this.CurrentItem.ID == int.Parse(Request.QueryString[PathData.ItemQueryKey]);
                return false;
            }
        }

        protected string ResolveUrl(object url)
        {
            return ResolveUrl(url as string);
        }

        protected new string ResolveUrl(string url)
        {
            return Engine.ManagementPaths.ResolveResourceUrl(url);
        }

        #region IItemContainer & IContentTemplate
        ContentItem IContentTemplate.CurrentItem
        {
            get { return CurrentItem; }
            set { CurrentItem = ItemUtility.EnsureType<TPart>(value); }
        }

        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }
        #endregion
    }
}
