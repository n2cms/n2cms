using System.Linq;
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
            foreach (ContentItem item in state.GetItemsToSave().Distinct())
            {
                if (item == state.Content)
                    continue;

                changer.ChangeTo(item, toState);
            }
        }
    }
}
