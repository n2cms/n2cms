using System;
using System.Collections.Generic;
using System.Text;

using Castle.Core;

using N2.Plugin;
using N2.Engine;
using N2.Web;
using N2.Persistence;
using N2.Templates.Items;

namespace N2.Templates.Services
{
	/// <summary>
	/// Makes sure the not found page is displayed whenever an url not leading 
	/// to a page is used.
	/// </summary>
	public class NotFoundHandler : IStartable, IAutoStart
	{
		IUrlParser parser;
		
		public NotFoundHandler(IUrlParser parser)
		{
			this.parser = parser;
		}

		void parser_PageNotFound(object sender, PageNotFoundEventArgs e)
		{
			AbstractStartPage startPage = parser.StartPage as AbstractStartPage;
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

        #region IAutoStart Members

        void IAutoStart.Start()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
