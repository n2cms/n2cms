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
    public class TargetingRadar : IAutoStart
    {
        public static bool Enabled { get; set; }

        public IEnumerable<DetectorBase> Detectors { get; private set; }

        public TargetingRadar(Configuration.HostSection config, DetectorBase[] detectors)
        {
            Enabled = config.Targeting.Enabled;
            if (!config.Targeting.Enabled || config.Targeting.IsCleared)
            {
                Detectors = Enumerable.Empty<DetectorBase>();
                return;
            }
            var removedDetectors = new HashSet<string>(config.Targeting.RemovedElements.Select(t => t.Name));
            var sortableDetectors = detectors.Where(d => !removedDetectors.Contains(d.Name)).ToList();
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

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
