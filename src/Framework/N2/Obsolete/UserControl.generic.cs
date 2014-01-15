using System;

namespace N2.Web.UI
{
    /// <summary>
    /// This class name has been deprecated. A user control base used to for quick access to content data.
    /// </summary>
    /// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
    [Obsolete("Please use N2.Web.UI.ContentUserControl instead.")]
    public abstract class UserControl<TPage> : ContentUserControl<TPage>
        where TPage : N2.ContentItem
    {
    }
}
