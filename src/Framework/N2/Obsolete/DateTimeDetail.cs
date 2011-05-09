using System;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>A DateTime content detail. A number of content details can be associated with one content item.</summary>
	[Serializable]
	[DebuggerDisplay("{Name}: DateTimeValue: {DateTimeValue}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class DateTimeDetail : ContentDetail
	{
    }
}
