namespace N2.Edit.Workflow.Commands
{
    public class MoveToPositionCommand : CommandBase<CommandContext>
    {
        public override void Process(CommandContext state)
        {
            if(state.Content.Parent == null)
                return;

            if(state.Parameters.ContainsKey("MoveBefore"))
                MoveTo(state, "MoveBefore", 0);
            else if(state.Parameters.ContainsKey("MoveAfter"))
                MoveTo(state, "MoveAfter", 1);
        }

        private void MoveTo(CommandContext state, string parameterName, int offset)
        {
            if (!state.Parameters.ContainsKey(parameterName) || state.Parameters[parameterName] == null)
                return;

            string relativeToPath = (string)state.Parameters[parameterName];
            var siblings = state.Content.Parent.Children;

            int index = siblings.Count - 1;
            for (int i = 0; i < siblings.Count; i++)
            {
                if(siblings[i].Path == relativeToPath)
                {
                    index = i;
                    break;
                }
            }
            Utility.MoveToIndex(siblings, state.Content, index + offset);
            foreach (var updatedItem in Utility.UpdateSortOrder(siblings))
            {
                state.RegisterItemToSave(updatedItem);
            }
        }
    }
}
