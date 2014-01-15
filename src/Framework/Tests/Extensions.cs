using System;

namespace N2.Tests
{
    public static class Extensions
    {
        public static DateTime StripMilliseconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
    }
}
