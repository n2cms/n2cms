using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// Base class for actions that can be scheduled to be executed by the 
    /// system at certain intervals. Inherit from this class and use the 
    /// [ScheduleExecution] attribute to enable.
    /// </summary>
    public abstract class ScheduledAction
    {
        public abstract void Execute();

        private TimeSpan interval = new TimeSpan(0, 1, 0);
        DateTime lastExecuted = DateTime.MinValue;
        Repeat repeat = Repeat.Once;
        bool isExecuting = false;

        public bool IsExecuting
        {
            get { return isExecuting; }
            set { isExecuting = value; }
        }

        public TimeSpan Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public DateTime LastExecuted
        {
            get { return lastExecuted; }
            set { lastExecuted = value; }
        }

        public Repeat Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }
    }
}
