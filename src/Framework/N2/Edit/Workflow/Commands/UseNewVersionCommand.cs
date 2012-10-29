using N2.Persistence;
using N2.Edit.Versioning;

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
            if (versionMaker.IsVersionable(state.Content) && state.Content.ID != 0)
            {
                state.Content = versionMaker.AddVersion(state.Content, asPreviousVersion: false);
            }
        }
    }
}
