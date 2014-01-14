namespace N2.Edit.Workflow.Commands
{
    public class RedirectCommand : CommandBase<CommandContext>
    {
        public string Url { get; set; }

        public RedirectCommand(string url)
        {
            Url = url;
        }

        public override void Process(CommandContext ctx)
        {
            ctx["RedirectTo"] = Url;
        }
    }
}
