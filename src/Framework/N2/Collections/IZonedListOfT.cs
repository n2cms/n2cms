using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
	public interface IZonedList<T> where T : class, IPlaceable
	{
		IList<T> FindPages();
		IList<T> FindParts();
		IList<T> FindParts(string zoneName);
		IList<string> FindZoneNames();
	}
}
