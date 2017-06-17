using System;
using System.Linq;

namespace N2.Edit.Workflow.Commands
{
    public class UseMasterCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
			UseMasterPartsToSaveRecursive(state, state.Content);
            state.Content = state.Content.VersionOf;
        }

		private void UseMasterPartsToSaveRecursive(CommandContext state, ContentItem content)
		{
			foreach (var child in content.Children.FindParts().Where(ci => ci.VersionOf.HasValue))
			{
				state.UnregisterItemToSave(child);
				state.RegisterItemToSave(child.VersionOf);
				UseMasterPartsToSaveRecursive(state, child);
			}
		}
	}
}
