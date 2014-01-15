using System;

namespace N2.Persistence
{
    /// <summary>Used to mark that should not be copied.</summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field)]
    internal class DoNotCopyAttribute : System.Attribute
    {
    }
}
