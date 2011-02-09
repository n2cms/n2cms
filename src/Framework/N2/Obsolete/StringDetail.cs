using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>
	/// A string content detail. A number of content details can be associated 
	/// with one content item.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: StringValue: {StringValue}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class StringDetail : ContentDetail
	{
	}
}