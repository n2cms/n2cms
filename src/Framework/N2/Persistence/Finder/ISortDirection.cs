using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence.Finder
{
	public interface ISortDirection
	{
		IQueryEnding Asc { get; }
		IQueryEnding Desc { get; }
	}
}
