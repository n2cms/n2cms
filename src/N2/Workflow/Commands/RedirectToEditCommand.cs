using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit;

namespace N2.Workflow.Commands
{
    public class RedirectToEditCommand : CommandBase<CommandContext>
    {
        IEditManager editManager;

        public RedirectToEditCommand(IEditManager editManager)
        {
            this.editManager = editManager;
        }

        public override void Process(CommandContext state)
        {
            state.RedirectTo = editManager.GetEditExistingItemUrl(state.Data);
        }
    }
}
