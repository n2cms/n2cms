using System;
using N2.Engine;

namespace N2.Edit.Installation
{

    [Service]
    public class DatabaseStatusCache
    {
        InstallationManager installer;
        bool isUpAndRunning = false;
        SystemStatusLevel level = SystemStatusLevel.Unknown;

        public DatabaseStatusCache(InstallationManager installer)
        {
            this.installer = installer;
        }

        public virtual SystemStatusLevel GetStatus()
        {
            if (isUpAndRunning)
                return level;

            var previousLevel = level;

            var status = installer.GetStatus();
            level = status.Level;

            if (level >= SystemStatusLevel.UpAndRunning)
                isUpAndRunning = true;
            
            if (previousLevel != level && DatabaseStatusChanged != null)
                DatabaseStatusChanged(this, new EventArgs());

            return level;
        }

        public event EventHandler DatabaseStatusChanged;
    }
}
