using System;

namespace N2.Plugin
{
    public class PluginInitializationException : Exception
    {
        public PluginInitializationException(string message, Exception[] innerExceptions)
            : base(message, innerExceptions[0])
        {
            this.innerExceptions = innerExceptions;
        }

        private Exception[] innerExceptions;

        public Exception[] InnerExceptions
        {
            get { return innerExceptions; }
            set { innerExceptions = value; }
        }
    }
}
