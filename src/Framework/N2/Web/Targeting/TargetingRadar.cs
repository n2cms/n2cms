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
		public IEnumerable<DetectorBase> Detectors { get; private set; }

		public TargetingRadar(DetectorBase[] detectors)
		{
			var sortableDetectors = detectors.ToList();
			sortableDetectors.Sort();
			Detectors = sortableDetectors;
		}

		public virtual TargetingContext BuildTargetingContext(HttpContextBase context)
		{
			var ctx = new TargetingContext(context);

			foreach (var detector in Detectors)
				if (detector.IsTarget(ctx))
					ctx.TargetedBy.Add(detector);

			return ctx;
		}
	}
}
