using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin.Scheduling;

namespace N2.Tests.Plugin.Scheduling
{
	[ScheduleExecution(Repeat.Once)]
	public class OnceAction : ScheduledAction
	{
		public int Executions = 0;
		public DateTime LastCall;

		public override void Execute()
		{
			Executions++;
			LastCall = N2.Utility.CurrentTime();
		}
	}

	[ScheduleExecution(60, Repeat = Repeat.Indefinitely)]
	public class RepeatAction : ScheduledAction
	{
		public int Executions = 0;
		public DateTime LastCall;

		public override void Execute()
		{
			Executions++;
			LastCall = N2.Utility.CurrentTime();
		}
	}
}
