using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Security;

namespace N2.Workflow.Commands
{
    public class IntentCommand : CommandBase<CommandContext>
    {
        Permission intent;
        public IntentCommand(Permission intent)
        {
            this.intent = intent;
        }
        public override void Process(CommandContext state)
        {
            state.Intent = intent;
        }
    }
}
