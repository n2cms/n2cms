
namespace N2.Edit.Workflow.Commands
{
    public class UpdateObjectCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            state.Binder.UpdateObject(state);
        }
    }
}
