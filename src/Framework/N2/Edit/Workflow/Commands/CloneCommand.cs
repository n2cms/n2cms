namespace N2.Edit.Workflow.Commands
{
    public class CloneCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            var clone = state.Content.Clone(false);
            clone.VersionOf = state.Content.VersionOf;
            state.Content = clone;
        }
    }
}
