using N2.Persistence.Search;
namespace N2.Definitions
{
    /// <summary>
    /// Marks a page containing text content.
    /// </summary>
    public interface IContentPage : IPage
    {
        /// <summary>The title of the page.</summary>
        string Title { get; set; }

        /// <summary>The text content of the page.</summary>
        string Text { get; set; }
    }
}
