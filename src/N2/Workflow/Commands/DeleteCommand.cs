using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;

namespace N2.Workflow.Commands
{
    public class DeleteCommand : CommandBase<CommandContext>
    {
        IRepository<int, ContentItem> repository;

        public DeleteCommand(IRepository<int, ContentItem> repository)
        {
            this.repository = repository;
        }

        public override void Process(CommandContext state)
        {
            repository.Delete(state.Content);
        }
    }
}
