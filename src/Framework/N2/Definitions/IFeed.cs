using System;
using System.Collections.Generic;
using N2.Persistence.Search;
using N2.Persistence;

namespace N2.Definitions
{
    /// <summary>
    /// Items implementing this interface provide feed meta data and syndicated 
    /// items.
    /// </summary>
    public interface IFeed
    {
        string Title { get; set; }
        string Url { get; }
        DateTime? Published { get; }
        int NumberOfItems { get; set; }
        string Tagline { get; set; }
        string Author { get; set; }
        
        IEnumerable<ISyndicatable> GetItems();
    }
}
