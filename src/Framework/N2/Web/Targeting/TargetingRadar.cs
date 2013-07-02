using N2.Engine;
using N2.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Web.Targeting
{
	[Service]
	public class TargetingRadar
	{
		private DetectorBase[] detectors;

		public TargetingRadar(DetectorBase[] detectors)
		{
			this.detectors = detectors;
		}

		public virtual TargetingContext BuildTargetingContext(HttpContextBase context)
		{
			var ctx = new TargetingContext(context);

			foreach (var detector in detectors)
				detector.AppendFlags(ctx);

			return ctx;
		}
	}
}
