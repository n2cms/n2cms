using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Provides quick acccess to often used APIs.
	/// </summary>
	public class ControllerContentHelper : ContentHelperBase
	{
		public ControllerContentHelper(IEngine engine, Func<PathData> pathGetter)
			: base(engine, pathGetter)
		{
		}
	}
}
