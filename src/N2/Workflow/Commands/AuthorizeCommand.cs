using N2.Edit;
using N2.Security;

namespace N2.Workflow.Commands
{
    public class AuthorizeCommand : CommandBase<CommandContext>
    {
        ISecurityManager security;

        public AuthorizeCommand(ISecurityManager security)
        {
            this.security = security;
        }

        public override void Process(CommandContext ctx)
        {
            if (!security.IsAuthorized(ctx.User, ctx.Data, ctx.Intent))
            {
                ctx.ValidationErrors.Add(new ValidationError("Unauthorized", "Not authorized to " + ctx.Intent));
                throw new StopExecutionException();
            }
        }
    }
}
