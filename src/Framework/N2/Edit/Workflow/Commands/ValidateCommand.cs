
namespace N2.Edit.Workflow.Commands
{
    public class ValidateCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            state.ValidationErrors = state.Validator.Validate(state);
            if (state.ValidationErrors != null && state.ValidationErrors.Count > 0)
                throw new StopExecutionException();
        }
    }
}
