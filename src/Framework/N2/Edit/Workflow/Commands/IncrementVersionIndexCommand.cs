using System.Linq;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class IncrementVersionIndexCommand : CommandBase<CommandContext>
    {
        IVersionManager versionProvider;

        public IncrementVersionIndexCommand(IVersionManager versionProvider)
        {
            this.versionProvider = versionProvider;
        }

        public override void Process(CommandContext state)
        {
            var masterVersion = state.Content.VersionOf ?? state.Content;
            var versions = versionProvider.GetVersionsOf(masterVersion);
            if (versions.Count > 1)
            {
                int greatestIndex = versions.Max(v => v.VersionIndex);
                state.Content.VersionIndex = greatestIndex + 1;
            }
            else
                state.Content.VersionIndex = 0;
        }
    }
}
