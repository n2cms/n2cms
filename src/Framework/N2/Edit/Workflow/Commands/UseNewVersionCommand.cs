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
			if(versionMaker.IsVersionable(state.Content))
				state.Content = versionMaker.AddVersion(state.Content, asPreviousVersion: false);

			//if (versionMaker.IsVersionable(state.Content))
			//{
			//    if (state.Content.IsPage)
			//        state.Content = versionMaker.AddVersion(state.Content, asPreviousVersion: false);
			//    else
			//    {
			//        var page = Find.ClosestPage(state.Content);
			//        var pageVersion = versionMaker.AddVersion(state.Content, asPreviousVersion: false);
			//        state.Content = pageVersion.FindPartVersion(state.Content);
			//    }
			//}
        }
    }
}
