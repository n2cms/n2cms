using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Edit
{
	public enum CreationPosition
	{
		/// <summary>Before the selected item on the same level.</summary>
		Before,
		/// <summary>Below the selected item, one level down.</summary>
		Below,
		/// <summary>After the selected item one level down.</summary>
		After
	}
}
