using System;

namespace N2.Engine
{
    /// <summary>
    /// Resolves content adapter for a given type at runtime.
    /// </summary>
    public interface IContentAdapterProvider
    {
        /// <summary>Resolves the adapter for the current type.</summary>
        /// <returns>A suitable adapter for the given type.</returns>
        T ResolveAdapter<T>(Type contentType) where T : AbstractContentAdapter;

        /// <summary>Resolves the adapter for the current item.</summary>
        /// <returns>A suitable adapter for the given item.</returns>
        T ResolveAdapter<T>(ContentItem item) where T : AbstractContentAdapter;
    }
}
