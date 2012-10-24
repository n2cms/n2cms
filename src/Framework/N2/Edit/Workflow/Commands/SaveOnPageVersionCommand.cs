using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Versioning;

namespace N2.Edit.Workflow.Commands
{
	public class SaveOnPageVersionCommand : CommandBase<CommandContext>
	{
		private IVersionManager versionMaker;

		public SaveOnPageVersionCommand(IVersionManager versionMaker)
		{
			this.versionMaker = versionMaker;
		}

		public override void Process(CommandContext state)
		{
			var part = state.Content;
			var page = Find.ClosestPage(part);
			var pageVersion = page.VersionOf.HasValue
				? page
				: versionMaker.AddVersion(page, asPreviousVersion: false);
			var parentVersion = pageVersion.FindPartVersion(part.Parent);

			if (state.Parameters.ContainsKey("MoveBeforeVersionKey") && !string.IsNullOrEmpty(state.Parameters["MoveBeforeVersionKey"] as string))
			{
				var beforeKey = (string)state.Parameters["MoveBeforeVersionKey"];
				var beforeItem = pageVersion.FindDescendantByVersionKey(beforeKey);
				beforeItem.Parent.InsertChildBefore(part, beforeItem.SortOrder);
			}
			else if (state.Parameters.ContainsKey("MoveBeforeSortOrder") && !string.IsNullOrEmpty(state.Parameters["MoveBeforeSortOrder"] as string))
			{
				int beforeSortOrder = Convert.ToInt32(state.Parameters["MoveBeforeSortOrder"]);
				parentVersion.InsertChildBefore(part, beforeSortOrder);
			}
			else
			{
				part.AddTo(parentVersion);
				Utility.UpdateSortOrder(parentVersion.Children);
			}

			versionMaker.UpdateVersion(pageVersion);
		}
	}
}
