using System;
using System.Diagnostics;
using System.Timers;
using N2.Engine;
using N2.Linq;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// A wrapper for a timer that beats at a certain interval.
    /// </summary>
	[Service(typeof(IHeart))]
    public class Heart : IAutoStart, IHeart
    {
		private readonly Engine.Logger<Heart> logger;
        Timer timer;

        public Heart()
        {
            timer = new Timer(60 * 1000);
        }

        public Heart(ConnectionMonitor connection, Configuration.EngineSection config)
        {
            if (config.Scheduler.Interval < 1) throw new ArgumentException("Cannot beat at a pace below 1 per second. Set engine.scheduler.interval to at least 1.", "config");

            timer = new Timer(config.Scheduler.Interval * 1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			connection.Online += delegate { timer.Start(); };
			connection.Offline += delegate { timer.Stop(); };
        }

		/// <summary>Occurs when a time unit has elapsed.</summary>
		public event EventHandler Beat;

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			logger.Debug("Beat: " + DateTime.Now);
			if (Beat != null)
				Beat(this, e);
		}

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
