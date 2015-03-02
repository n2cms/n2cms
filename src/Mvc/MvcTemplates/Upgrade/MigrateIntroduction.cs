using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Web.Parts;
using N2.Engine;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Upgrade
{
    [Service(typeof(AbstractMigration))]
    public class MigrateIntroduction : AbstractMigration
    {
        private IPersister persister;

        public MigrateIntroduction(IPersister persister)
        {
            this.persister = persister;
            this.Title = "Migrate introduction to summary on news and event items";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            try
            {
	            return persister.Repository.Count(new Parameter("class", "News")) > 0;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private IEnumerable<ContentItem> GetNews()
        {
            return persister.Repository.Find(new Parameter("class", "News"));
        }

        private IEnumerable<ContentItem> GetEvents()
        {
            return persister.Repository.Find(new Parameter("class", "Event"));
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var events = GetNews().Union(GetEvents()).ToList();
            var result = new MigrationResult(this); ;
            using (var tx = persister.Repository.BeginTransaction())
            {
                foreach (var ev in events.OfType<ContentPageBase>())
                {
                    var intro = ev.GetDetail<string>("Introduction", null);
                    if (intro != null)
                    {
                        ev.SetDetail("Introduction", null, typeof(string));
                        ev.Summary = intro;
                        persister.Save(ev);
                        result.UpdatedItems++;
                    }
                }
                tx.Commit();
                return result;
            }
        }
    }
}
