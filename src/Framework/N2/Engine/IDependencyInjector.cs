using System;

namespace N2.Engine
{
    /// <summary>
    /// Injects dependencies onto content items as they are created.
    /// </summary>
    public interface IDependencyInjector
    {
        bool FulfilDependencies(ContentItem item);
    }
}
