using N2.Persistence;
using N2.Edit.Versioning;

namespace N2.Edit.Workflow.Commands
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
