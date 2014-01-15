using System.Diagnostics;
using N2.Plugin.Scheduling;

namespace N2.Edit.Trash
{
    /// <summary>
    /// Cleans the recycle bin.
    /// </summary>
    [ScheduleExecution(24, TimeUnit.Hours)]
    public class TrashCleanupAgent : ScheduledAction
    {
        public override void Execute()
        {
            ITrashHandler handler = Engine.Resolve<ITrashHandler>();
            handler.PurgeOldItems();
        }
    }
}
