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
            if (!state.Content.Published.HasValue)
                state.Content.Published = Utility.CurrentTime();

            if (state.Content.Published.Value > Utility.CurrentTime())
                state.Content.Published = Utility.CurrentTime();
        }
    }
}
