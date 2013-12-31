using System;
using Castle.Core;

using N2.Plugin;
using N2.Web;
using N2.Templates.Items;

namespace N2.Templates.Services
{
    /// <summary>
    /// Makes sure the not found page is displayed whenever an url not leading 
    /// to a page is used.
    /// </summary>
    public class NotFoundHandler : IAutoStart
    {
        IUrlParser parser;
        
        public NotFoundHandler(IUrlParser parser, IWebContext context)
        {
            this.parser = parser;
        }

        void parser_PageNotFound(object sender, PageNotFoundEventArgs e)
        {
            StartPage startPage = parser.StartPage as StartPage;
            if (startPage != null && startPage.NotFoundPage != null && !e.Url.StartsWith("edit/", StringComparison.InvariantCultureIgnoreCase))
            {
                e.AffectedItem = startPage.NotFoundPage;
            }
        }

        public void Start()
        {
            parser.PageNotFound += parser_PageNotFound;
        }

        public void Stop()
        {
            parser.PageNotFound -= parser_PageNotFound;
        }
    }
}
