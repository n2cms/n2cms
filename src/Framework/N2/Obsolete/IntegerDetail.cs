using System;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>
    /// An integer content detail. A number of content details can be 
    /// associated with one content item.
    /// </summary>
	[Serializable]
	[DebuggerDisplay( "{Name}: IntValue: {IntValue}")]
	[Obsolete("Use ContentDetail instead", true)]
	public class IntegerDetail : ContentDetail
	{
    }
}
