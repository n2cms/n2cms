using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Workflow
{
    public class CommandProcessEventArgs : EventArgs
    {
        public CommandBase<CommandContext> Command { get; set; }

        public CommandContext Context { get; set; }
    }
}
