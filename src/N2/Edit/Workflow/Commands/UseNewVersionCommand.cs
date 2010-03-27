using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class UseNewVersionCommand : CommandBase<CommandContext>
    {
        IVersionManager versionMaker;
        public UseNewVersionCommand(IVersionManager versionMaker)
        {
            this.versionMaker = versionMaker;
        }
        public override void Process(CommandContext state)
        {
			if(versionMaker.IsVersionable(state.Content))
				state.Content = versionMaker.SaveVersion(state.Content);
        }
    }
}
