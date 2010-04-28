using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Details
{
	/// <summary>
	/// Tells the system when to make urls in a detail relative or absolute.
	/// </summary>
	public enum RelativityMode
	{
		Never,
		ExportRelativeImportAbsolute,
	}
}
