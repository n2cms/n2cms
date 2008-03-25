using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Items
{
	public abstract class AbstractRootPage : AbstractContentPage, N2.Web.ISitesProvider
	{
		#region ISitesProvider Members

		public ICollection<N2.Web.Site> GetSites()
		{
			List<N2.Web.Site> sites = new List<N2.Web.Site>();
			foreach (AbstractStartPage page in GetChildren<AbstractStartPage>())
				sites.Add(new N2.Web.Site(this.ID, page.ID, page.HostName));
			return sites;
		}

		#endregion
	}
}
