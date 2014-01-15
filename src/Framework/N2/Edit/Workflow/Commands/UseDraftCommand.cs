using N2.Persistence;
using N2.Edit.Versioning;

namespace N2.Edit.Workflow.Commands
{
    public class UseDraftCommand : CommandBase<CommandContext>
    {
        IVersionManager versionMaker;
        
        public UseDraftCommand(IVersionManager versionMaker)
        {
            this.versionMaker = versionMaker;
        }

        public override void Process(CommandContext state)
        {
            if (versionMaker.IsVersionable(state.Content) && (state.Content.State == ContentState.Published || state.Content.State == ContentState.Unpublished))
            {
                state.Content = versionMaker.AddVersion(state.Content, asPreviousVersion: false);
            }
        }
    }
}
