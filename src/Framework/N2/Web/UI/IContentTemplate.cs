namespace N2.Web.UI
{
    /// <summary>
    /// This interface is used for setting the content item associated with a 
    /// template (ascx or aspx).
    /// </summary>
    public interface IContentTemplate
    {
        ContentItem CurrentItem { get; set; }
    }
}
