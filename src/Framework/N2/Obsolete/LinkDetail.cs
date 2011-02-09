using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>
	/// A link detail used to relate items to each other.
	/// </summary>
	[DebuggerDisplay("{Name}: LinkedItem: {LinkedItem}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class LinkDetail : ContentDetail
	{
	}
}