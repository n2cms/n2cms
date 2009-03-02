using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
	public class ZoneController : IZoneController
	{
		#region IAspectController Members

		public PathData Path { get; set; }

		public N2.Engine.IEngine Engine { get; set; }

		#endregion
	}
}
