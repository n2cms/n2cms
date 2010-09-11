using System;
using System.Collections.Generic;
namespace N2.Collections
{
	public interface IPipeline
	{
		IEnumerable<ContentItem> Pipe(IEnumerable<ContentItem> previous);
	}
}
