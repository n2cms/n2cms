using System;
using System.Collections.Generic;
using System.Text;

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
