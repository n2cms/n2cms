using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit;
using N2.Web;

namespace N2.Workflow.Commands
{
    public class RedirectToPreviewCommand : CommandBase<CommandContext>
    {
        IEditManager editManager;

        public RedirectToPreviewCommand(IEditManager editManager)
        {
            this.editManager = editManager;
        }

        public override void Process(CommandContext state)
        {
            state.RedirectTo = editManager.GetPreviewUrl(state.Data);
            if (state.Data.VersionOf != null)
                state.RedirectTo = Url.Parse(state.RedirectTo)
                    .AppendQuery("preview", state.Data.ID)
                    .AppendQuery("original", state.Data.VersionOf.ID);
        }
    }
}
