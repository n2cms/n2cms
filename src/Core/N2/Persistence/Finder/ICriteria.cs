using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence.Finder
{
	/// <summary>
	/// Classes implementing this interface provides functionality to add 
	/// criteras for equality.
	/// </summary>
	public interface ICriteria<T>
	{
		/// <summary>Equal to.</summary>
		IQueryAction Eq(T value);

		/// <summary>Not equal to.</summary>
		IQueryAction NotEq(T value);

		IQueryAction In(params T[] anyOf);
	}
}
