using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
	public interface IPageableList<T>
	{
		/// <summary>Gets a subset of the list without causing the complete list to be loaded.</summary>
		/// <param name="skip">The number of items to skip.</param>
		/// <param name="take">The number of items to take.</param>
		/// <returns>A list of items within the given range.</returns>
		IList<T> FindRange(int skip, int take);
	}
}
