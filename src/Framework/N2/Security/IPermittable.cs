using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Security
{
	/// <summary>
	/// When implemented by a plugin this attribute controls who may use it.
	/// </summary>
	public interface IPermittable
	{
		/// <summary>The permissions required.</summary>
		Permission RequiredPermission { get; set; }
	}
}
