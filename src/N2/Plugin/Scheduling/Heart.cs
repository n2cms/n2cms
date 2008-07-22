using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using System.Timers;
using System.Diagnostics;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// A wrapper for a timer that beats at a certain interval.
    /// </summary>
    public class Heart : IStartable, IAutoStart, IHeart
    {
        Timer timer;

        public Heart()
        {
            timer = new Timer(60 * 1000);
        }

        public Heart(Configuration.EngineSection config)
        {
            if (config.Scheduler.Interval < 1) throw new ArgumentException("Cannot beat at a pace below 1 per second. Set engine.scheduler.interval to at least 1.", "config");

            timer = new Timer(config.Scheduler.Interval * 1000);
        }

        public event EventHandler Beat;

        public void Start()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Beat: " + DateTime.Now);
            if (Beat != null)
                Beat(this, e);
        }

        public void Stop()
        {
            timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
            timer.Stop();
        }
    }
}
