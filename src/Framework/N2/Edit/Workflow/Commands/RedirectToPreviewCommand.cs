using N2.Engine;
using N2.Web;

namespace N2.Edit.Workflow.Commands
{
    public class RedirectToPreviewCommand : CommandBase<CommandContext>
    {
        IContentAdapterProvider adapters;

        public RedirectToPreviewCommand(IContentAdapterProvider adapters)
        {
            this.adapters = adapters;
        }

        public override void Process(CommandContext state)
        {
            string redirectTo = adapters.ResolveAdapter<NodeAdapter>(state.Content).GetPreviewUrl(state.Content);
            if (state.Content.VersionOf.HasValue)
                redirectTo = Url.Parse(redirectTo)
                    .AppendQuery("preview", state.Content.VersionIndex)
                    .AppendQuery("original", state.Content.VersionOf.ID);
            state["RedirectTo"] = redirectTo;
        }
    }
}
