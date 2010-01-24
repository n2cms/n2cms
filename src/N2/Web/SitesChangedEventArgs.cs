using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
	public class SitesChangedEventArgs : EventArgs
	{
		public Site PreviousDefault { get; set; }
		public Site CurrentDefault { get; set; }
		public Site[] PreviousSites { get; set; }
		public Site[] CurrentSites { get; set; }
	}
}
