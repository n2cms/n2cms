using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Versioning;

namespace N2.Edit.Workflow.Commands
{
	public class SaveOnPageVersionCommand : CommandBase<CommandContext>
	{
		private Persistence.IVersionManager versionMaker;

		public SaveOnPageVersionCommand(Persistence.IVersionManager versionMaker)
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
			part.AddTo(parentVersion);

			versionMaker.UpdateVersion(pageVersion);
		}
	}
}
