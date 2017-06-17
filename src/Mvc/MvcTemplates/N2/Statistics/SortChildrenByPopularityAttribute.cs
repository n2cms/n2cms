using N2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	public class SortChildrenByPopularityAttribute : SortChildrenAttribute
	{
		public SortChildrenByPopularityAttribute()
			: base(typeof(PopularityChildrenSorter))
		{
		}
	}
}