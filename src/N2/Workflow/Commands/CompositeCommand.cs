using System.Collections.Generic;
using System.Linq;
using N2.Persistence;

namespace N2.Workflow.Commands
{
    public class CompositeCommand : CommandBase<CommandContext>
    {
        IRepository<int, ContentItem> repository;

        public CompositeCommand(IRepository<int, ContentItem> repository, string title, params CommandBase<CommandContext>[] commands)
        {
            this.repository = repository;
            Title = title;
            Commands = commands;
        }

        public IEnumerable<CommandBase<CommandContext>> Commands { get; set; }

        public override string Name
        {
            get { return string.Join(",", Commands.Select(c => c.Name).ToArray()); }
        }

        public override void Process(CommandContext context)
        {
            using (var tx = repository.BeginTransaction())
            {
                foreach (var command in Commands)
                {
                    command.Process(context);
                }
                tx.Commit();
            }
        }
    }
}
