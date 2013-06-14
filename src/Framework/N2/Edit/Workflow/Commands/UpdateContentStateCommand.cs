using System.Linq;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class UpdateContentStateCommand : CommandBase<CommandContext>
    {
        StateChanger changer;
        ContentState toState;
    		IPersister persister;

        public UpdateContentStateCommand(StateChanger changer, ContentState toState, IPersister persister)
        {
            this.changer = changer;
            this.toState = toState;
						this.persister = persister;
        }

        public override void Process(CommandContext state)
        {
            changer.ChangeTo(state.Content, toState);
			foreach (ContentItem item in state.GetItemsToSave().Distinct())
			{
				persister.SaveAsDraft(item);

				if (item == state.Content)
					continue;

				changer.ChangeTo(item, toState);
			}
        }
    }
}
