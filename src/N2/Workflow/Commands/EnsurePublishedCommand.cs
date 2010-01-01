using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Workflow.Commands
{
    public class EnsurePublishedCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            if (!state.Data.Published.HasValue)
                state.Data.Published = Utility.CurrentTime();

            if (state.Data.Published.Value > Utility.CurrentTime())
                state.Data.Published = Utility.CurrentTime();
        }
    }
}
