using System.Collections.Generic;

namespace N2.Edit.Installation
{
    public class MigrationResult
    {
        public MigrationResult(AbstractMigration migration)
        {
            Migration = migration;
            Errors = new List<string>();
        }

        public AbstractMigration Migration { get; set; }
        public IList<string> Errors { get; set; }
        public int UpdatedItems { get; set; }
        public string RedirectTo { get; set; }
    }

    public abstract class AbstractMigration
    {
        public AbstractMigration()
        {
            Title = GetType().Name;
            Description = "";
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public virtual bool IsApplicable(DatabaseStatus status)
        {
            return true;
        }

        public abstract MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus);

        public bool? TryApplicable(DatabaseStatus initialStatus)
        {
            try
            {
                return IsApplicable(initialStatus);
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
