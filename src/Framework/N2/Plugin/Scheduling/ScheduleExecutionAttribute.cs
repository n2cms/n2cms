using System;
using N2.Engine;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// Defines how often a scheduled action should execute.
    /// </summary>
    public class ScheduleExecutionAttribute : ServiceAttribute
    {
        private string name = null;
        private int sortOrder = int.MaxValue;
        private Type decorates;
        private int interval = 60 * 60;
        private TimeUnit unit = TimeUnit.Seconds;
        private Repeat repeat = Repeat.Indefinitely;

        protected ScheduleExecutionAttribute()
            : base (typeof(ScheduledAction))
        {
        }

        public ScheduleExecutionAttribute(Repeat repeat)
            : this()
        {
            this.repeat = repeat;
        }

        public ScheduleExecutionAttribute(int seconds)
            : this()
        {
            interval = seconds;
        }

        public ScheduleExecutionAttribute(int interval, TimeUnit unit)
            : this()
        {
            this.interval = interval;
            this.unit = unit;
        }

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public TimeUnit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public Repeat Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }

        public Type Decorates
        {
            get { return decorates; }
            set { decorates = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

        public bool IsAuthorized(System.Security.Principal.IPrincipal user)
        {
            return true;
        }

        public TimeSpan CalculateInterval()
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
					return new TimeSpan(0, interval, 0);
				case TimeUnit.Hours:
					return new TimeSpan(interval, 0, 0);
				case TimeUnit.Days:
					return new TimeSpan(interval, 0, 0, 0);
                default:
                    throw new NotImplementedException("Unknown time unit: " + unit);
            }
        }
    }
}
