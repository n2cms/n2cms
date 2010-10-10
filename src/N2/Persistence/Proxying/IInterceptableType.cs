using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Marks a type available for interception.
	/// </summary>
	public interface IInterceptableType
	{
		/// <summary>The actual content type the proxied object represents.</summary>
		/// <returns>The actual type.</returns>
		Type GetContentType();

		/// <summary>Sets a value to the underlying detail store.</summary>
		/// <param name="detailName">The name of the detail.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="valueType">The type of value to assign.</param>
		void SetDetail(string detailName, object value, Type valueType);

		/// <summary>Gets a value from the underlying detail store.</summary>
		/// <param name="detailName">The name of the detail.</param>
		/// <returns>The value of the given detial name of null.</returns>
		object GetDetail(string detailName);
	}
}
