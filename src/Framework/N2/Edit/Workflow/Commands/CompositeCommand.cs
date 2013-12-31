using System.Linq;

namespace N2.Edit.Workflow.Commands
{
    public class CompositeCommand : CommandBase<CommandContext>
    {
        public CompositeCommand(string title, params CommandBase<CommandContext>[] commands)
        {
            Title = title;
            Commands = commands;
        }

        public CommandBase<CommandContext>[] Commands { get; set; }

        public override string Name
        {
            get { return string.Join(",", Commands.Select(c => c.Name).ToArray()); }
        }

        public override void Process(CommandContext context)
        {
            foreach (var command in Commands)
            {
                command.Process(context);
            }
        }
    }
}
