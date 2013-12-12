using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class ActiveContentSaveCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            ((IActiveContent)state.Content).Save();
        }
    }
}
