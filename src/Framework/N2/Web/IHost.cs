using System;
using System.Collections.Generic;

namespace N2.Web
{
    /// <summary>
    /// Classes implementing this interface knows about available 
    /// <see cref="Site">Sites</see> and which one is the current
    /// based on the context.
    /// </summary>
    public interface IHost
    {
        /// <summary>The current site based on the request's host header information. Fallbacks to defualt site.</summary>
        Site CurrentSite { get; }

        /// <summary>The default site if the current cannot be determined.</summary>
        Site DefaultSite { get; set; }

        /// <summary>All sites in the system.</summary>
        IList<Site> Sites { get; }

        /// <summary>Gets the site associated with an url.</summary>
        /// <param name="hostUrl">The url of the site.</param>
        /// <returns>The associated site or null if no matching site is found.</returns>
        Site GetSite(Url hostUrl);

        /// <summary>Gets the site associated with an item.</summary>
        /// <param name="item">The item whose site to get.</param>
        /// <returns>The site this node belongs to.</returns>
        Site GetSite(ContentItem item);

        /// <summary>Gets the site with the given start page ID.</summary>
        /// <param name="startPageId">The id of the site's start page.</param>
        /// <returns>The site or null if no start page with that id exists.</returns>
        Site GetSite(int startPageId);

        /// <summary>Adds sites to the available sites.</summary>
        /// <param name="additionalSites">Sites to add.</param>
        void AddSites(IEnumerable<Site> additionalSites);

        /// <summary>Replaces the site list with new sites.</summary>
        /// <param name="defaultSite">The default site to use.</param>
        /// <param name="newSites">The new site list.</param>
        void ReplaceSites(Site defaultSite, IEnumerable<Site> newSites);

        /// <summary>Determines whether an item is a start page.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is a configured start page.</returns>
        bool IsStartPage(ContentItem item);

        /// <summary>Is triggered when the sites collection changes.</summary>
        event EventHandler<SitesChangedEventArgs> SitesChanged;
    }
}
