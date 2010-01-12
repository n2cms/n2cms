using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;

namespace N2.Workflow.Commands
{
    public class ReplaceMasterCommand : CommandBase<CommandContext>
    {
        IVersionManager versionMaker;
        public ReplaceMasterCommand(IVersionManager versionMaker)
        {
            this.versionMaker = versionMaker;
        }
        public override void Process(CommandContext state)
        {
            versionMaker.ReplaceVersion(state.Content.VersionOf, state.Content, true);
        }
    }
}
