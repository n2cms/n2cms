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
    public class MigrateHomeRedirect : AbstractMigration
    {
        private IPersister persister;

        public MigrateHomeRedirect(IPersister persister)
        {
            this.persister = persister;
            this.Title = "Removes the redirect to home which is now included in the menu by default.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            try
            {
                return persister.Repository.Find(new Parameter("class", "Redirect")).Any(p => p.Parent is Models.Pages.LanguageRoot);
            }
            catch (Exception)
            {
                return true;
            }
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            using (var tx = persister.Repository.BeginTransaction())
            {
                var result = new MigrationResult(this); ;
				foreach (var redirect in persister.Repository.Find(new Parameter("class", "Redirect")).Where(p => p.Parent is Models.Pages.LanguageRoot))
                {
                    persister.Delete(redirect);
                    result.UpdatedItems++;
                }
                tx.Commit();
                return result;
            }
        }
    }
}
