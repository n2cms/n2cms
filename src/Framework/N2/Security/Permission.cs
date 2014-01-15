using System;

namespace N2.Security
{
    [Flags]
    public enum Permission
    {
        None = 0,
        Read = 1,
        Write = 2,
        Publish = 4,
        Administer = 8,
        ReadWrite = Read | Write,
        ReadWritePublish = Read | Write | Publish,
        Full = Read | Write | Publish | Administer
    }
}
