using System;
namespace N2.Web.Mvc
{
    /// <summary>
    /// An interface that can be used to identify an N2 view.
    /// </summary>
    public interface IContentView
    {
        /// <summary>Provides access to a simplified API to access data.</summary>
        DynamicContentHelper Content { get; set; }
    }
}
