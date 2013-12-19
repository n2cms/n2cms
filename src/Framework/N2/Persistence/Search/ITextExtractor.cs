using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Extracts text from content that can be indexed.
    /// </summary>
    public interface ITextExtractor
    {
        IEnumerable<IndexableField> Extract(ContentItem item);
    }
}
