using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Workflow.Commands
{
    public class ValidateCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            state.ValidationErrors = state.Validator.Validate(state.Data);
            if (state.ValidationErrors != null && state.ValidationErrors.Count > 0)
                throw new StopExecutionException();
        }
    }
}
