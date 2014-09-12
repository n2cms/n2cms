using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Web.Parts;
using N2.Engine;

namespace N2.Templates.Mvc.Upgrade
{
    [Service(typeof(AbstractMigration))]
    public class MigrateTopParts : AbstractMigration
    {
        private IPersister persister;

        public MigrateTopParts(IPersister persister)
        {
            this.persister = persister;
            this.Title = "Remove top parts and store data on the containing page";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            try
            {
                return persister.Repository.Find(new Parameter("class", "Top")).Any(p => p.State != ContentState.Deleted);
            }
            catch (Exception)
            {
                return true;
            }
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var parts = persister.Repository.Find(new Parameter("class", "Top")).ToList();
            var result = new MigrationResult(this); ;
            using (var tx = persister.Repository.BeginTransaction())
            {
                foreach (var part in parts.Where(p => p.State != ContentState.Deleted).Where(p => p.Parent != null))
                {
                    part.Parent.StoreEmbeddedPart("Header", part);
                    persister.Save(part.Parent);
                    persister.Delete(part);
                    result.UpdatedItems++;
                }
                tx.Commit();
                return result;
            }
        }
    }
}
