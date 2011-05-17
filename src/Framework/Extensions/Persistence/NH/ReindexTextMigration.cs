using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Installation;
using N2.Details;

namespace N2.Persistence.NH
{
	[N2.Engine.Service(typeof(AbstractMigration))]
	public class ReindexTextMigration : AbstractMigration
	{
		ISessionProvider sessionProvider;

		public ReindexTextMigration(ISessionProvider sessionProvider)
		{
			this.sessionProvider = sessionProvider;

			Title = "Reindex all content using the lucene based index";
			Description = "Will re-save all items which triggers indexing using lucene search database format.";
		}

		public override bool IsApplicable(DatabaseStatus status)
		{
			return status.DatabaseVersion < 4;
		}

		public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
		{
			var ftSession = sessionProvider.OpenSession.FullText();
			ftSession.PurgeAll(typeof(ContentItem));
			ftSession.PurgeAll(typeof(ContentDetail));
			int count = 0;
			foreach (var item in ftSession.CreateQuery("from ContentItem").List<ContentItem>())
			{
				ftSession.Index(item);
				count++;
			}
			return new MigrationResult(this) { UpdatedItems = count };
		}
	}
}
