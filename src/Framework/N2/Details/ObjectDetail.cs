using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>A content detail. A number of content details can be associated with one content item.</summary>
	[Serializable]
	[DebuggerDisplay("{Name}: Value: {Value}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class ObjectDetail : ContentDetail
	{
	}
}