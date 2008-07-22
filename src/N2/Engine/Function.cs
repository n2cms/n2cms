using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Engine
{
    public delegate ReturnT Function<ReturnT>();
    public delegate ReturnT Function<T1, ReturnT>(T1 argument);
}
