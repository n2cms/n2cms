using N2.Plugin.Scheduling;

namespace N2.Tests.Plugin.Scheduling
{
    [ScheduleExecution(60, Repeat = Repeat.Indefinitely)]
    public class RepeatAction : OnceAction
    {
    }
}
