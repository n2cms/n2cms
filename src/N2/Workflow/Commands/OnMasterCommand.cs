using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Workflow.Commands
{
    public static class On
    {
        public static CommandBase<CommandContext> Master(CommandBase<CommandContext> command)
        {
            return new MasterCommand(command);
        }

        public class MasterCommand : CommandBase<CommandContext>
        {
            CommandBase<CommandContext> command;
            public MasterCommand(CommandBase<CommandContext> command)
            {
                this.command = command;
            }

            public override void Process(CommandContext state)
            {
                var incoming = state.Data;
                state.Data = state.Data.VersionOf;
                try
                {
                    command.Process(state);
                }
                finally
                {
                    state.Data = incoming;
                }
            }
        }
    }
}
