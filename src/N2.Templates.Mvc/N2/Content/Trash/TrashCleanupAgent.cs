using System;
using System.Collections.Generic;
using System.Web;
using N2.Plugin.Scheduling;
using N2.Collections;
using System.Diagnostics;

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
            if (Debugger.IsAttached)
                return;

			ITrashHandler handler = Engine.Resolve<ITrashHandler>();
			handler.PurgeOldItems();
		}
	}
}
