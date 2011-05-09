using System;
using N2.Plugin.Scheduling;

namespace N2.Tests.Plugin.Scheduling
{
    [ScheduleExecution(Repeat.Once)]
    public class OnceAction : ScheduledAction
    {
        public int executions = 0;
        public DateTime LastCall;

        public override void Execute()
        {
            executions++;
            LastCall = N2.Utility.CurrentTime();
        }
    }
}
