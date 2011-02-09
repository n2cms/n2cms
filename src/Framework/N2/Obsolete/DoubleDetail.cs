using System;
using System.Collections;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>
    /// An double content detail. A number of content details can be associated 
    /// with one content item.
    /// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: DoubleValue: {DoubleValue}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class DoubleDetail : ContentDetail
	{
    }
}
