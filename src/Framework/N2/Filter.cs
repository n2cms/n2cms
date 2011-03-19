using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Edit.Workflow;

namespace N2
{
	/// <summary>
	/// Provides access to common filters.
	/// </summary>
	public static class Filter
	{
		public static FilterHelper Is
		{
			get { return new FilterHelper(); }
		}
	}
}
