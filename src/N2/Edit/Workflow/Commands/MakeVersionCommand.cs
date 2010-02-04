using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class MakeVersionCommand : CommandBase<CommandContext>
    {
        IVersionManager versionMaker;
        public MakeVersionCommand(IVersionManager versionMaker)
        {
            this.versionMaker = versionMaker;
        }
        public override void Process(CommandContext state)
        {
            if (state.Content.GetType().GetCustomAttributes(typeof(N2.Persistence.NotVersionableAttribute), true).Length > 0)
                return;
            versionMaker.SaveVersion(state.Content);
        }
    }

}
