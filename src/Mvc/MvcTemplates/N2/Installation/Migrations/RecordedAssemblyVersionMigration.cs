using N2.Edit.Installation;
using N2.Management.Installation;
using N2.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace N2.Installation.Migrations
{
    [Migration]
    public class RecordedAssemblyVersionMigration : AbstractMigration
    {
        private IPersister persister;
        
        public RecordedAssemblyVersionMigration(IPersister persister)
        {
            this.persister = persister;
            Title = "Record Assembly Version";
            Description = "Records current N2 assembly version for future usage";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            try
            {
                return status.RootItem != null && status.RootItem["RecordedAssemblyVersion"] == null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var root = persister.Get(preSchemaUpdateStatus.RootItemID);
            root[InstallationManager.installationAssemblyVersion] = typeof(Context).Assembly.GetName().Version.ToString();
            root[InstallationManager.installationFileVersion] = InstallationUtility.GetFileVersion(typeof(Context).Assembly);
            root.RecordInstalledFeature("CMS");

            persister.Save(root);
            return new MigrationResult(this) { UpdatedItems = 1 };
        }
    }
}
