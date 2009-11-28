using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence.Finder
{
	public interface IOrderBy
	{
		ISortDirection ID { get; }
		ISortDirection Parent { get; }

		ISortDirection Title { get; }
		ISortDirection Name { get; }
		ISortDirection ZoneName { get; }
		ISortDirection Created { get; }
		ISortDirection Updated { get; }
		ISortDirection Published { get; }
		ISortDirection Expires { get; }
        ISortDirection SortOrder { get; }
        ISortDirection VersionIndex { get; }
        ISortDirection Visible { get; }
		ISortDirection SavedBy { get; }

		ISortDirection Detail(string name);
	}
}
