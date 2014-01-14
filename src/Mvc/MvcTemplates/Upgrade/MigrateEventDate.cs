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
    public class MigrateEventDate : AbstractMigration
    {
        private IPersister persister;

        public MigrateEventDate(IPersister persister)
        {
            this.persister = persister;
            this.Title = "Migrate data about event date on events";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            try
            {
                return GetEvents().Any();
            }
            catch (Exception)
            {
                return true;
            }
        }

        private IEnumerable<ContentItem> GetEvents()
        {
            return persister.Repository.Find(new Parameter("class", "Event"));
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var events = GetEvents().ToList();
            var result = new MigrationResult(this); ;
            using (var tx = persister.Repository.BeginTransaction())
            {
                foreach (var ev in events.OfType<Event>())
                {
                    ev.EventDate = ev.GetDetail<DateTime?>("EventDate", null);
                    persister.Save(ev);
                    result.UpdatedItems++;
                }
                tx.Commit();
                return result;
            }
        }
    }
}
