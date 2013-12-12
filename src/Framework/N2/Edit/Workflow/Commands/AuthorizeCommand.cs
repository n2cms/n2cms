using N2.Security;

namespace N2.Edit.Workflow.Commands
{
    public class AuthorizeCommand : CommandBase<CommandContext>
    {
        ISecurityManager security;
        Permission requiredPermission;

        public AuthorizeCommand(ISecurityManager security, Permission requiredPermission)
        {
            this.security = security;
            this.requiredPermission = requiredPermission;
        }

        public override void Process(CommandContext ctx)
        {
            if (!security.IsAuthorized(ctx.User, ctx.Content, requiredPermission))
            {
                ctx.ValidationErrors.Add(new ValidationError("Unauthorized", "Not authorized to " + requiredPermission));
                throw new StopExecutionException();
            }
        }
    }
}
