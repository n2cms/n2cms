using N2.Edit.Installation;

namespace N2.Tests.Fakes
{
    public class FakeDatabaseStatusCache : DatabaseStatusCache
    {
        public FakeDatabaseStatusCache(InstallationManager installer)
            : base(installer)
        {
        }

        public SystemStatusLevel level = SystemStatusLevel.UpAndRunning;
        public override SystemStatusLevel GetStatus()
        {
            return level;
        }
    }
}
