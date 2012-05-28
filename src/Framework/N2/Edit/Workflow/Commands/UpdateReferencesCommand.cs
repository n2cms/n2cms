using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;

namespace N2.Edit.Workflow.Commands
{
	public class UpdateReferencesCommand : CommandBase<CommandContext>
	{
		public override void Process(CommandContext state)
		{
			if (state.Original.State != ContentState.New && state.Original.Name != state.Content.Name)
			{
				state.RedirectUrl = new SelectionUtility(state.Content, null).SelectedUrl("{ManagementUrl}/Content/LinkTracker/UpdateReferences.aspx").ToUrl().AppendQuery("previousParent", state.Content.Parent.Path).AppendQuery("previousName", state.Original.Name);
			}
		}
	}
}
