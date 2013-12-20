using N2.Plugin.Scheduling;

namespace N2.Addons.MyAddon.Services
{
    /// <summary>
    /// Place long-running or asynchronous tasks in a scheduled action in a 
    /// scheduled action.
    /// </summary>
    [ScheduleExecution(1, TimeUnit.Minutes)]
    public class MyScheduledAction : ScheduledAction
    {
        public override void Execute()
        {
            // Notice how we can resolve the component we registered in the initializer
            N2.Context.Current.Resolve<MyComponent>().VisitParts();

            // We can change the interval for subsequent executions, note that these settings are not persisted across applications restarts
            Interval = Interval + Interval; 
        }
    }
}
