using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;

namespace N2.Workflow.Commands
{
    public class SaveCommand : CommandBase<CommandContext>
    {
        IPersister persister;
        public SaveCommand(IPersister persister)
        {
            this.persister = persister;
        }

        public override void Process(CommandContext state)
        {
            persister.Save(state.Data);
        }
    }
}
