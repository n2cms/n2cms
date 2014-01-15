using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Wiki
{
    public interface IArticle
    {
        DateTime? Published { get; }
        DateTime Updated { get; }
        string Title { get; }
        string Text { get; }
        string Url { get; }
        string SavedBy { get; }
        
        IWiki WikiRoot { get; }
        object this[string detailName] { get; }
    }
}
