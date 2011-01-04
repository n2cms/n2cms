using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Workflow.Commands
{
    public class UseMasterCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            state.Content = state.Content.VersionOf;
        }
    }
}
