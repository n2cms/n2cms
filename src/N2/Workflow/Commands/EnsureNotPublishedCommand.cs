using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Workflow.Commands
{
    public class EnsureNotPublishedCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            if (state.Data.Published.HasValue)
                state.Data.Published = null;
        }
    }
}
