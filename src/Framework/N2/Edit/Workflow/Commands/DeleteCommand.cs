using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class DeleteCommand : CommandBase<CommandContext>
    {
        IRepository<ContentItem> repository;

        public DeleteCommand(IRepository<ContentItem> repository)
        {
            this.repository = repository;
        }

        public override void Process(CommandContext state)
        {
            repository.Delete(state.Content);
            state.UnregisterItemToSave(state.Content);
        }
    }
}
