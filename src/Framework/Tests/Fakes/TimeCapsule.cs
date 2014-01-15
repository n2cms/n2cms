using System;

namespace N2.Tests.Fakes
{
    /// <summary>
    /// Fakes the time from N2.Utility.CurrentTime
    /// </summary>
    public class TimeCapsule : IDisposable
    {
        Func<DateTime> backup;

        public TimeCapsule(DateTime time)
        {
            backup = N2.Utility.CurrentTime;
            N2.Utility.CurrentTime = () => time;
        }
        public void Dispose()
        {
            N2.Utility.CurrentTime = backup;
        }
    }
}
