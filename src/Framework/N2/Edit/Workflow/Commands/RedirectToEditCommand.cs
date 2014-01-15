namespace N2.Edit.Workflow.Commands
{
    public class RedirectToEditCommand : CommandBase<CommandContext>
    {
        IEditUrlManager editManager;

        public RedirectToEditCommand(IEditUrlManager editManager)
        {
            this.editManager = editManager;
        }

        public override void Process(CommandContext state)
        {
            state["RedirectTo"] = editManager.GetEditExistingItemUrl(state.Content);
        }
    }
}
