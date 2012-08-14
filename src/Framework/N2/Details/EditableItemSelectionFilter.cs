using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Details
{
	[Flags]
	public enum EditableItemSelectionFilter
	{
		None = 0,
		Pages = 1,
		Parts = 2,
		All = Pages | Parts,
	}
}
