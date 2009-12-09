using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Workflow.Commands
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
            ctx.RedirectTo = Url;
        }
    }
}
