using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki
{
    public interface IArticle
    {
        DateTime? Published { get; }
        DateTime Updated { get; }
        string Title { get; }
        string Text { get; }
        string SavedBy { get; }
        string Action { get; }
        string ActionParameter { get; }
        
        IWiki WikiRoot { get; }
        object this[string detailName] { get; }
    }
}
