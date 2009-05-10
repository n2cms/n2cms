using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Addons.Tagging
{
	public interface ITag
	{
		string Title { get; }
		int ReferenceCount { get; }
	}
}
