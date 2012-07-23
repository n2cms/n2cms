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
			if (state.Content.IsPage 
				&& state.Original.State != ContentState.New 
				&& state.Original.Name != state.Content.Name)
			{
				var url = new SelectionUtility(state.Content, null).SelectedUrl("{ManagementUrl}/Content/LinkTracker/UpdateReferences.aspx").ToUrl().AppendQuery("previousName", state.Original.Name);
				if (state.Content.Parent != null)
					url = url.AppendQuery("previousParent", state.Content.Parent.Path);
				state.RedirectUrl = url;
			}
		}
	}
}
