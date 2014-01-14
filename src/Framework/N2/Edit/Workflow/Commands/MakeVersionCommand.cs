using N2.Persistence;
using N2.Edit.Versioning;

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
            if (versionMaker.IsVersionable(state.Content))
                versionMaker.AddVersion(state.Content, asPreviousVersion : true);
        }
    }

}
