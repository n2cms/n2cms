using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
	/// <summary>
	/// This interface is used by the <see cref="DynamicSitesProvider"/> to 
	/// retrieve available sites.
	/// </summary>
	public interface ISitesSource
	{
		IEnumerable<Site> GetSites();
	}
}
