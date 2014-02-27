using System;

namespace N2.Plugin
{
    public class SomeException : Exception
    {
        public SomeException(string message)
            : base(message)
        {
        }
    }
}
