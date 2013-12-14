using System;

namespace N2.Edit
{
    /// <summary>
    /// Service providing content items based on an external family and key.
    /// </summary>
    public interface IExternalContentRepository
    {
        /// <summary>Gets an external content item or creates if it doesn't already exist.</summary>
        /// <param name="familyKey">The group of external items in which to look for the key.</param>
        /// <param name="key">The key of the content item to look for.</param>
        /// <param name="url">The url on which the content item is displayed.</param>
        /// <param name="contentType">The type of content item to crete when no existing item in place.</param>
        /// <returns>A dynamically created content item that is used to associate information.</returns>
        ContentItem GetOrCreate(string familyKey, string key, string url, Type contentType = null);
    }
}
