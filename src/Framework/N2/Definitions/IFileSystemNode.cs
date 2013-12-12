using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    public interface IFileSystemNode : ISystemNode
    {
        string LocalUrl { get; }
    }
}
