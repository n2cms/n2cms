using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit;
using N2.Web;

namespace N2.Edit.Workflow.Commands
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
            string redirectTo = editManager.GetPreviewUrl(state.Content);
            if (state.Content.VersionOf != null)
                redirectTo = Url.Parse(redirectTo)
                    .AppendQuery("preview", state.Content.ID)
                    .AppendQuery("original", state.Content.VersionOf.ID);
			state["RedirectTo"] = redirectTo;
        }
    }
}
