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

			if (state.Parameters.ContainsKey("MoveBeforeSortOrder"))
			{
				int beforeSortOrder = Convert.ToInt32(state.Parameters["MoveBeforeSortOrder"]);
				bool wasAdded = false;
				for (int i = 0; i < parentVersion.Children.Count; i++)
				{
					var sibling = parentVersion.Children[i];
					if (sibling.SortOrder >= beforeSortOrder)
					{
						parentVersion.Children.Insert(i, part);
						Utility.UpdateSortOrder(parentVersion.Children);
						wasAdded = true;
						break;
					}
				}
				if (!wasAdded)
				{
					part.AddTo(parentVersion);
					Utility.UpdateSortOrder(parentVersion.Children);
				}
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
