using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Security;
using N2.Persistence;
using N2.Edit;
using N2.Web;

namespace N2.Edit.Workflow.Commands
{
    public class UpdateContentStateCommand : CommandBase<CommandContext>
    {
        StateChanger changer;
        ContentState toState;

        public UpdateContentStateCommand(StateChanger changer, ContentState toState)
        {
            this.changer = changer;
            this.toState = toState;
        }

        public override void Process(CommandContext state)
        {
            changer.ChangeTo(state.Content, toState);
        }
    }
}
