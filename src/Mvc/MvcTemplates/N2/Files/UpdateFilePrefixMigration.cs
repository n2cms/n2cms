using N2.Configuration;
using N2.Edit.Installation;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Files
{
	[N2.Engine.Service(typeof(AbstractMigration))]
	public class UpdateFilePrefixMigration : AbstractMigration
	{
		private EditSection config;

		public UpdateFilePrefixMigration(EditSection config)
		{
			this.config = config;
			Title = "Updates links to files with prefix";
			Description = "Updates links to files within an upload folder prepending the configured prefix (if any)";
		}

		public override bool IsApplicable(DatabaseStatus status)
		{
			return config.UploadFolders.AllElements.Any(uf => !string.IsNullOrEmpty(uf.UrlPrefix));
		}

		public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
		{
			var path = config.UploadFolders.AllElements.Where(uf => !string.IsNullOrEmpty(uf.UrlPrefix)).Select(uf => uf.Path).FirstOrDefault();
			path = Url.ToAbsolute(path);

			return new MigrationResult(this) 
			{ 
				RedirectTo = "{ManagementUrl}/Content/LinkTracker/UpdateReferences.aspx"
					+ "?selectedUrl=" + path 
					+ "&previousUrl=" +  path
					+ "&location=upgrade"
			};
		}
	}
}