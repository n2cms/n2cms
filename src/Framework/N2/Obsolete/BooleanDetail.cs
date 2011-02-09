using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>
	/// A boolean content detail. A number of content details can be associated 
	/// with one content item.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: BoolValue: {BoolValue}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class BooleanDetail : ContentDetail
	{
	}
}