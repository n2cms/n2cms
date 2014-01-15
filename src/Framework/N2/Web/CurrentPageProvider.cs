using System.Collections.Generic;
using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// Provides the current page routed through an url.
    /// </summary>
    public class CurrentPageProvider : IProvider<ContentItem>
    {
        IUrlParser parser;

        public CurrentPageProvider(IUrlParser parser)
        {
            this.parser = parser;
        }

        #region IProvider<ContentItem> Members

        public ContentItem Get()
        {
            return parser.CurrentPage;
        }

        public IEnumerable<ContentItem> GetAll()
        {
            return new[] { parser.CurrentPage };
        }

        #endregion
    }
}
