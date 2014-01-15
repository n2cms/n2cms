namespace N2.Edit.Workflow.Commands
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
                var incoming = state.Content;
                state.Content = state.Content.VersionOf;
                try
                {
                    command.Process(state);
                }
                finally
                {
                    state.Content = incoming;
                }
            }
        }
    }
}
