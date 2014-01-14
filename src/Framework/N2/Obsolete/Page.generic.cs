using System;

namespace N2.Web.UI
{
    /// <summary>
    /// This class name has been deprecated. Page base class that provides easy access to the current page item.
    /// </summary>
    /// <typeparam name="TPage">The type of content item served by the page inheriting this class.</typeparam>
    [Obsolete("Please use N2.Web.UI.ContentPage instead.")]
    public abstract class Page<TPage> : ContentPage<TPage>
        where TPage : N2.ContentItem
    {
    }
}
