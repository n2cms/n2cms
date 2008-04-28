using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace N2.Edit.Trash
{
	public interface ITrashCan
	{
		bool Enabled { get; }
		IList<ContentItem> Children { get; }
	}
}
