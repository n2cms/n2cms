using System;
using N2.Engine;

namespace N2.Edit
{
    /// <summary>
    /// Management helper methods.
    /// </summary>
    internal static class ManagementUtility
    {
        /// <summary>Shorthand for resolving an adapter.</summary>
        /// <typeparam name="T">The type of adapter to get.</typeparam>
        /// <param name="engine">Used to resolve the provider.</param>
        /// <param name="item">The item whose adapter to get.</param>
        /// <returns>The most relevant adapter.</returns>
        internal static T GetContentAdapter<T>(this IEngine engine, ContentItem item) where T : AbstractContentAdapter
        {
            Type itemType = item != null ? item.GetContentType() : typeof (ContentItem);
            return engine.Resolve<IContentAdapterProvider>().ResolveAdapter<T>(itemType);
        }
    }
}
