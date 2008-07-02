using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki
{
    public interface IArticle
    {
        string Title { get; }
        string Text { get; }
        string SavedBy { get; }
        DateTime? Published { get; }
        ContentItem WikiRoot { get; }
    }
}
