namespace N2.Edit.Workflow.Commands
{
    public class EnsureNotPublishedCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            state.Content.Published = null;
        }
    }
}
