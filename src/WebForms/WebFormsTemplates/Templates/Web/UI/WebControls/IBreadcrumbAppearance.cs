namespace N2.Templates.Web.UI.WebControls
{
    /// <summary>
    /// When applied to a content item this interface helps the path control to 
    /// determine the item appearance in the <see cref="Path"/> control.
    /// </summary>
    public interface IBreadcrumbAppearance : N2.Web.ILink
    {
        bool VisibleInBreadcrumb { get; }
    }
}
